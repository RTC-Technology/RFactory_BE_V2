using AutoMapper;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Quality.Services
{
    public class QualitySpecificationService : IQualitySpecificationService
    {
        private readonly IRepository<Entities.QualitySpecification> _repo;
        private readonly IRepository<Entities.QualitySpecificationItem> _item;
        private readonly IRepository<Entities.QualitySpecificationProduct> _product;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QualitySpecificationService(
            IRepository<Entities.QualitySpecification> repo,
            IRepository<Entities.QualitySpecificationItem> item,
            IRepository<Entities.QualitySpecificationProduct> product,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _item = item;
            _product = product;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<QualitySpecificationDto>> CreateAsync(QualitySpecificationRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.SpecificationCode == request.SpecificationCode, ct);
            if (existing is not null)
            {
                return Result<QualitySpecificationDto>.Failure($"Quality specification '{request.SpecificationCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.QualitySpecification>(request);
            var items = request.QualitySpecificationItems ?? new List<QualitySpecificationItemRequest>();
            var products = request.QualitySpecificationProducts ?? new List<QualitySpecificationProductRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _item.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);

                await _product.AddRange(products.Select(product => ToProductEntity(product, entity.Id)).ToList(), token);

                return Result<QualitySpecificationDto>.Success(_mapper.Map<QualitySpecificationDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Quality specification {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _item.Where(p => p.QualitySpecificationId == id, ct);
            var products = await _product.Where(p => p.QualitySpecificationId == id, ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _item.DeleteRange(items, token);
                await _product.DeleteRange(products, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<QualitySpecificationDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<QualitySpecificationDto>>(await _repo.GetAll(ct));

        public async Task<QualitySpecificationDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<QualitySpecificationDto>(entity);
        }

        public async Task<Result<QualitySpecificationDto>> UpdateAsync(ulong id, QualitySpecificationRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<QualitySpecificationDto>.Failure($"Quality specification {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.SpecificationCode == request.SpecificationCode, ct);
            if (existing is not null)
            {
                return Result<QualitySpecificationDto>.Failure($"Quality specification '{request.SpecificationCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _item.Where(l => l.QualitySpecificationId == id, ct);
            var items = request.QualitySpecificationItems;
            var keptItemIds = (items ?? new List<QualitySpecificationItemRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptItemIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<QualitySpecificationDto>.Failure($"Item(s) {string.Join(", ", foreignItems)} do not belong to Quality specification {id}.");
            }

            var storedProducts = await _product.Where(l => l.QualitySpecificationId == id, ct);
            var products = request.QualitySpecificationProducts;
            var keptProductIds = (products ?? new List<QualitySpecificationProductRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignProducts = keptProductIds.Where(lineId => storedProducts.All(s => s.Id != lineId)).ToList();
            if (foreignProducts.Count > 0)
            {
                return Result<QualitySpecificationDto>.Failure($"Product(s) {string.Join(", ", foreignProducts)} do not belong to Quality specification {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _item.DeleteRange(storedItems.Where(s => !keptItemIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _item.Update(target, token);
                    }

                    await _item.AddRange(items.Where(l => l.Id == 0).Select(line => ToItemEntity(line, id)).ToList(), token);
                }

                if (products is not null)
                {
                    await _product.DeleteRange(storedProducts.Where(s => !keptProductIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in products.Where(l => l.Id != 0))
                    {
                        var target = storedProducts.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _product.Update(target, token);
                    }

                    await _product.AddRange(products.Where(l => l.Id == 0).Select(line => ToProductEntity(line, id)).ToList(), token);
                }

                return Result<QualitySpecificationDto>.Success(_mapper.Map<QualitySpecificationDto>(entity));
            }, ct);
        }

        private Entities.QualitySpecificationItem ToItemEntity(QualitySpecificationItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.QualitySpecificationItem>(line);
            entity.QualitySpecificationId = id;
            return entity;
        }
        private Entities.QualitySpecificationProduct ToProductEntity(QualitySpecificationProductRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.QualitySpecificationProduct>(line);
            entity.QualitySpecificationId = id;
            return entity;
        }
    }

    public class QualitySpecificationItemService: IQualitySpecificationItemService
    {
        private readonly IRepository<Entities.QualitySpecificationItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QualitySpecificationItemService(
            IRepository<Entities.QualitySpecificationItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<QualitySpecificationItemDto>> CreateAsync(QualitySpecificationItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.QualitySpecificationItem>(request);
            await _repository.Add(entity, ct);
            return Result<QualitySpecificationItemDto>.Success(_mapper.Map<QualitySpecificationItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Quality specification item {id} was not found.");
        }

        public async Task<List<QualitySpecificationItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<QualitySpecificationItemDto>>(await _repository.GetAll(ct));
        public async Task<QualitySpecificationItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<QualitySpecificationItemDto>(entity);
        }

        public async Task<Result<QualitySpecificationItemDto>> UpdateAsync(ulong id, QualitySpecificationItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<QualitySpecificationItemDto>.Failure($"Quality specification item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<QualitySpecificationItemDto>.Success(_mapper.Map<QualitySpecificationItemDto>(entity));
        }
    }

    public class QualitySpecificationProductService : IQualitySpecificationProductService
    {
        private readonly IRepository<Entities.QualitySpecificationProduct> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QualitySpecificationProductService(
            IRepository<Entities.QualitySpecificationProduct> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<QualitySpecificationProductDto>> CreateAsync(QualitySpecificationProductRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.QualitySpecificationProduct>(request);
            await _repository.Add(entity, ct);
            return Result<QualitySpecificationProductDto>.Success(_mapper.Map<QualitySpecificationProductDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Quality specification product {id} was not found.");
        }

        public async Task<List<QualitySpecificationProductDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<QualitySpecificationProductDto>>(await _repository.GetAll(ct));
        public async Task<QualitySpecificationProductDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<QualitySpecificationProductDto>(entity);
        }

        public async Task<Result<QualitySpecificationProductDto>> UpdateAsync(ulong id, QualitySpecificationProductRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<QualitySpecificationProductDto>.Failure($"Quality specification product {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<QualitySpecificationProductDto>.Success(_mapper.Map<QualitySpecificationProductDto>(entity));
        }
    }
}
