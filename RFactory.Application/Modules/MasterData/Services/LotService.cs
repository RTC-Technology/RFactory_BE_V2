using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities = RFactory.Infrastructure.Entities;


namespace RFactory.Application.Modules.MasterData.Services
{
    public class LotService : ILotService
    {
        private readonly IRepository<Entities.Lot> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LotService(
            IRepository<Entities.Lot> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LotDto>> CreateAsync(LotRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Lot>(request);
            await _repository.Add(entity, ct);
            return Result<LotDto>.Success(_mapper.Map<LotDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Lot {id} was not found.");
        }

        public async Task<List<LotDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<LotDto>>(await _repository.GetAll(ct));
        public async Task<LotDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<LotDto>(entity);
        }

        public async Task<Result<LotDto>> UpdateAsync(ulong id, LotRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<LotDto>.Failure($"Lot {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<LotDto>.Success(_mapper.Map<LotDto>(entity));
        }
    }

    public class LotRuleService : ILotRuleService
    {
        private readonly IRepository<Entities.LotRule> _repo;
        private readonly IRepository<Entities.ProductLotRule> _productLotRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LotRuleService(
            IRepository<Entities.LotRule> repo,
            IRepository<Entities.ProductLotRule> productLotRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _productLotRepo = productLotRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LotRuleDto>> CreateAsync(LotRuleRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.RuleCode == request.RuleCode, ct);
            if (existing is not null)
            {
                return Result<LotRuleDto>.Failure($"Lot Rule '{request.RuleCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.LotRule>(request);
            var productLots = request.ProductLotRules ?? new List<ProductLotRuleRequest>();
            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _productLotRepo.AddRange(productLots.Select(productLot => ToProductEntity(productLot, entity.Id)).ToList(), token);

                return Result<LotRuleDto>.Success(_mapper.Map<LotRuleDto>(entity));
            }, ct);
        }

        

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Lot Rule {id} was not found.");
            }

            //var receiptId = (long)id;
            var productLots = await _productLotRepo.Where(p => p.LotRuleId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _productLotRepo.DeleteRange(productLots, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<LotRuleDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<LotRuleDto>>(await _repo.GetAll(ct));

        public async Task<LotRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<LotRuleDto>(entity);
        }

        public async Task<Result<LotRuleDto>> UpdateAsync(ulong id, LotRuleRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<LotRuleDto>.Failure($"Lot Rule {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.RuleCode == request.RuleCode, ct);
            if (existing is not null)
            {
                return Result<LotRuleDto>.Failure($"Lot Rule '{request.RuleCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _productLotRepo.Where(l => l.LotRuleId == id, ct);
            var productLots = request.ProductLotRules;
            var keptIds = (productLots ?? new List<ProductLotRuleRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreigns = keptIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreigns.Count > 0)
            {
                return Result<LotRuleDto>.Failure($"Product Lot Rule(s) {string.Join(", ", foreigns)} do not belong to LotRule {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (productLots is not null)
                {
                    await _productLotRepo.DeleteRange(storedItems.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in productLots.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _productLotRepo.Update(target, token);
                    }

                    await _productLotRepo.AddRange(productLots.Where(l => l.Id == 0).Select(line => ToProductEntity(line, id)).ToList(), token);
                }

                return Result<LotRuleDto>.Success(_mapper.Map<LotRuleDto>(entity));
            }, ct);
        }

        private Entities.ProductLotRule ToProductEntity(ProductLotRuleRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.ProductLotRule>(line);
            entity.LotRuleId = id;
            return entity;
        }
    }

    public class LotRuleSequenceService : ILotRuleSequenceService
    {
        private readonly IRepository<Entities.LotRuleSequence> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LotRuleSequenceService(
            IRepository<Entities.LotRuleSequence> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LotRuleSequenceDto>> CreateAsync(LotRuleSequenceRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.LotRuleSequence>(request);
            await _repository.Add(entity, ct);
            return Result<LotRuleSequenceDto>.Success(_mapper.Map<LotRuleSequenceDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Lot Rule Sequence {id} was not found.");
        }

        public async Task<List<LotRuleSequenceDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<LotRuleSequenceDto>>(await _repository.GetAll(ct));
        public async Task<LotRuleSequenceDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<LotRuleSequenceDto>(entity);
        }

        public async Task<Result<LotRuleSequenceDto>> UpdateAsync(ulong id, LotRuleSequenceRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<LotRuleSequenceDto>.Failure($"Lot Rule Sequence {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<LotRuleSequenceDto>.Success(_mapper.Map<LotRuleSequenceDto>(entity));
        }
    }

    public class ProductLotRuleService : IProductLotRuleService
    {
        private readonly IRepository<Entities.ProductLotRule> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductLotRuleService(
            IRepository<Entities.ProductLotRule> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductLotRuleDto>> CreateAsync(ProductLotRuleRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.ProductLotRule>(request);
            await _repository.Add(entity, ct);
            return Result<ProductLotRuleDto>.Success(_mapper.Map<ProductLotRuleDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Product Lot Rule {id} was not found.");
        }

        public async Task<List<ProductLotRuleDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<ProductLotRuleDto>>(await _repository.GetAll(ct));
        public async Task<ProductLotRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<ProductLotRuleDto>(entity);
        }

        public async Task<Result<ProductLotRuleDto>> UpdateAsync(ulong id, ProductLotRuleRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<ProductLotRuleDto>.Failure($"Product Lot Rule {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<ProductLotRuleDto>.Success(_mapper.Map<ProductLotRuleDto>(entity));
        }
    }
}
