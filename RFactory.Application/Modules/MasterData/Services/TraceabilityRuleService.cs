using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Application.Modules.Quality.Services;
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
    public class TraceabilityRuleService: ITraceabilityRuleService
    {
        private readonly IRepository<Entities.TraceabilityRule> _repo;
        private readonly IRepository<Entities.TraceabilityRuleItem> _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TraceabilityRuleService(
            IRepository<Entities.TraceabilityRule> repo,
            IRepository<Entities.TraceabilityRuleItem> item,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<TraceabilityRuleDto>> CreateAsync(TraceabilityRuleRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.RuleCode == request.RuleCode, ct);
            if (existing is not null)
            {
                return Result<TraceabilityRuleDto>.Failure($"Traceability rule '{request.RuleCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.TraceabilityRule>(request);
            var items = request.TraceabilityRuleItems ?? new List<TraceabilityRuleItemRequest>();
            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _item.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);

                return Result<TraceabilityRuleDto>.Success(_mapper.Map<TraceabilityRuleDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Traceability rule {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _item.Where(p => p.TraceabilityRuleId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _item.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<TraceabilityRuleDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<TraceabilityRuleDto>>(await _repo.GetAll(ct));

        public async Task<TraceabilityRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<TraceabilityRuleDto>(entity);
        }

        public async Task<Result<TraceabilityRuleDto>> UpdateAsync(ulong id, TraceabilityRuleRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<TraceabilityRuleDto>.Failure($"Traceability rule {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.RuleCode == request.RuleCode, ct);
            if (existing is not null)
            {
                return Result<TraceabilityRuleDto>.Failure($"Traceability rule '{request.RuleCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _item.Where(l => l.TraceabilityRuleId == id, ct);
            var items = request.TraceabilityRuleItems;
            var keptIds = (items ?? new List<TraceabilityRuleItemRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<TraceabilityRuleDto>.Failure($"Item(s) {string.Join(", ", foreignItems)} do not belong to Traceability rule {id}.");
            }


            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _item.DeleteRange(storedItems.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _item.Update(target, token);
                    }

                    await _item.AddRange(items.Where(l => l.Id == 0).Select(line => ToItemEntity(line, id)).ToList(), token);
                }

                return Result<TraceabilityRuleDto>.Success(_mapper.Map<TraceabilityRuleDto>(entity));
            }, ct);
        }

        private Entities.TraceabilityRuleItem ToItemEntity(TraceabilityRuleItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.TraceabilityRuleItem>(line);
            entity.TraceabilityRuleId = id;
            return entity;
        }
    }

    public class TraceabilityRuleItemService : ITraceabilityRuleItemService
    {
        private readonly IRepository<Entities.TraceabilityRuleItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TraceabilityRuleItemService(
            IRepository<Entities.TraceabilityRuleItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<TraceabilityRuleItemDto>> CreateAsync(TraceabilityRuleItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.TraceabilityRuleItem>(request);
            await _repository.Add(entity, ct);
            return Result<TraceabilityRuleItemDto>.Success(_mapper.Map<TraceabilityRuleItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Traceability rule item {id} was not found.");
        }

        public async Task<List<TraceabilityRuleItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<TraceabilityRuleItemDto>>(await _repository.GetAll(ct));
        public async Task<TraceabilityRuleItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<TraceabilityRuleItemDto>(entity);
        }

        public async Task<Result<TraceabilityRuleItemDto>> UpdateAsync(ulong id, TraceabilityRuleItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<TraceabilityRuleItemDto>.Failure($"Traceability rule item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<TraceabilityRuleItemDto>.Success(_mapper.Map<TraceabilityRuleItemDto>(entity));
        }
    }

    public class TraceabilityTypeService : ITraceabilityTypeService
    {
        private readonly IRepository<Entities.TraceabilityType> _repository;
        private readonly IMapper _mapper;

        public TraceabilityTypeService(
            IRepository<Entities.TraceabilityType> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TraceabilityTypeDto>> CreateAsync(TraceabilityTypeRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.TraceCode == request.TraceCode, ct);
            if (existing is not null)
            {
                return Result<TraceabilityTypeDto>.Failure($"Traceability type code '{request.TraceCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.TraceabilityType>(request);
            await _repository.Add(entity, ct);
            return Result<TraceabilityTypeDto>.Success(_mapper.Map<TraceabilityTypeDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Traceability type {id} was not found.");
        }

        public async Task<List<TraceabilityTypeDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<TraceabilityTypeDto>>(await _repository.GetAll(ct));
        public async Task<TraceabilityTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<TraceabilityTypeDto>(entity);
        }

        public async Task<Result<TraceabilityTypeDto>> UpdateAsync(ulong id, TraceabilityTypeRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<TraceabilityTypeDto>.Failure($"Traceability type {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.TraceCode == request.TraceCode, ct);
            if (existing is not null)
            {
                return Result<TraceabilityTypeDto>.Failure($"Traceability type code '{request.TraceCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<TraceabilityTypeDto>.Success(_mapper.Map<TraceabilityTypeDto>(entity));
        }
    }
}
