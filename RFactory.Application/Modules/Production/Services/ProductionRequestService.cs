using AutoMapper;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Services
{
    public class ProductionRequestService: IProductionRequestService
    {
        private readonly IRepository<Entities.ProductionRequest> _repo;
        private readonly IRepository<Entities.ProductionRequestItem> _itemRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductionRequestService(
            IRepository<Entities.ProductionRequest> repo,
            IRepository<Entities.ProductionRequestItem> itemRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _itemRepo = itemRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductionRequestDto>> CreateAsync(ProductionRequestRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.RequestNo == request.RequestNo, ct);
            if (existing is not null)
            {
                return Result<ProductionRequestDto>.Failure($"Production request '{request.RequestNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.ProductionRequest>(request);
            var items = request.ProductionRequestItems ?? new List<ProductionRequestItemRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _itemRepo.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);

                return Result<ProductionRequestDto>.Success(_mapper.Map<ProductionRequestDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Production request {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _itemRepo.Where(p => p.ProductionRequestId == id, ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _itemRepo.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<ProductionRequestDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<ProductionRequestDto>>(await _repo.GetAll(ct));

        public async Task<ProductionRequestDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<ProductionRequestDto>(entity);
        }

        public async Task<Result<ProductionRequestDto>> UpdateAsync(ulong id, ProductionRequestRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<ProductionRequestDto>.Failure($"Production request {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.RequestNo == request.RequestNo, ct);
            if (existing is not null)
            {
                return Result<ProductionRequestDto>.Failure($"Production request '{request.RequestNo}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _itemRepo.Where(l => l.ProductionRequestId == id, ct);
            var items = request.ProductionRequestItems;
            var keptItemIds = (items ?? new List<ProductionRequestItemRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptItemIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<ProductionRequestDto>.Failure($"Item(s) {string.Join(", ", foreignItems)} do not belong to Production request {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _itemRepo.DeleteRange(storedItems.Where(s => !keptItemIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _itemRepo.Update(target, token);
                    }

                    await _itemRepo.AddRange(items.Where(l => l.Id == 0).Select(line => ToItemEntity(line, id)).ToList(), token);
                }


                return Result<ProductionRequestDto>.Success(_mapper.Map<ProductionRequestDto>(entity));
            }, ct);
        }

        private Entities.ProductionRequestItem ToItemEntity(ProductionRequestItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.ProductionRequestItem>(line);
            entity.ProductionRequestId = id;
            return entity;
        }
    }

    public class ProductionRequestItemService : IProductionRequestItemService
    {
        private readonly IRepository<Entities.ProductionRequestItem> _repository;
        private readonly IRepository<Entities.ProductionOrder> _orderRepo;
        private readonly IMapper _mapper;

        public ProductionRequestItemService(
            IRepository<Entities.ProductionRequestItem> repository,
            IRepository<Entities.ProductionOrder> orderRepo,
            IMapper mapper)
        {
            _repository = repository;
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        public async Task<Result<ProductionRequestItemDto>> CreateAsync(ProductionRequestItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ProductionRequestItem>(request);
            await _repository.Add(entity, ct);
            return Result<ProductionRequestItemDto>.Success(_mapper.Map<ProductionRequestItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Production request item {id} was not found.");
        }

        public async Task<List<ProductionRequestItemDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _repository.GetAll(ct);
            var dtos = _mapper.Map<List<ProductionRequestItemDto>>(entities);
            if (dtos.Count > 0)
            {
                var itemIds = dtos.Select(d => (ulong?)d.Id).ToList();
                var orders = await _orderRepo.Where(o => o.ProductionRequestItemId.HasValue && itemIds.Contains(o.ProductionRequestItemId.Value), ct);
                var orderMap = orders.GroupBy(o => o.ProductionRequestItemId!.Value)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.CreatedDate).First().OrderNo);

                foreach (var dto in dtos)
                {
                    if (orderMap.TryGetValue(dto.Id, out var orderNo))
                    {
                        dto.ProductionOrderNo = orderNo;
                    }
                }
            }
            return dtos;
        }

        public async Task<ProductionRequestItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null) return null;
            var dto = _mapper.Map<ProductionRequestItemDto>(entity);
            var order = await _orderRepo.FirstOrDefault(o => o.ProductionRequestItemId == id, ct);
            if (order != null)
            {
                dto.ProductionOrderNo = order.OrderNo;
            }
            return dto;
        }

        public async Task<Result<ProductionRequestItemDto>> UpdateAsync(ulong id, ProductionRequestItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<ProductionRequestItemDto>.Failure($"Production request item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<ProductionRequestItemDto>.Success(_mapper.Map<ProductionRequestItemDto>(entity));
        }
    }
}