using AutoMapper;
using RFactory.Application.Modules.Quality.DTOs;
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
    public class InspectionPlanService : IInspectionPlanService
    {
        private readonly IRepository<Entities.InspectionPlan> _repo;
        private readonly IRepository<Entities.InspectionItem> _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InspectionPlanService(
            IRepository<Entities.InspectionPlan> repo,
            IRepository<Entities.InspectionItem> item,
            IRepository<Entities.QualitySpecificationProduct> product,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<InspectionPlanDto>> CreateAsync(InspectionPlanRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.PlanCode == request.PlanCode, ct);
            if (existing is not null)
            {
                return Result<InspectionPlanDto>.Failure($"Inspection plan '{request.PlanCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.InspectionPlan>(request);
            var items = request.InspectionItems ?? new List<InspectionItemRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _item.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);

                return Result<InspectionPlanDto>.Success(_mapper.Map<InspectionPlanDto>(entity));
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
            var items = await _item.Where(p => p.InspectionPlanId == id, ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _item.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<InspectionPlanDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<InspectionPlanDto>>(await _repo.GetAll(ct));

        public async Task<InspectionPlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<InspectionPlanDto>(entity);
        }

        public async Task<Result<InspectionPlanDto>> UpdateAsync(ulong id, InspectionPlanRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<InspectionPlanDto>.Failure($"Inspection plan {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.PlanCode == request.PlanCode, ct);
            if (existing is not null)
            {
                return Result<InspectionPlanDto>.Failure($"Inspection plan '{request.PlanCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _item.Where(l => l.InspectionPlanId == id, ct);
            var items = request.InspectionItems;
            var keptItemIds = (items ?? new List<InspectionItemRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptItemIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<InspectionPlanDto>.Failure($"Item(s) {string.Join(", ", foreignItems)} do not belong to Inspection plan {id}.");
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

                return Result<InspectionPlanDto>.Success(_mapper.Map<InspectionPlanDto>(entity));
            }, ct);
        }

        private Entities.InspectionItem ToItemEntity(InspectionItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.InspectionItem>(line);
            entity.InspectionPlanId = id;
            return entity;
        }
    }

    public class InspectionItemService : IInspectionItemService
    {
        private readonly IRepository<Entities.InspectionItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InspectionItemService(
            IRepository<Entities.InspectionItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<InspectionItemDto>> CreateAsync(InspectionItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.InspectionItem>(request);
            await _repository.Add(entity, ct);
            return Result<InspectionItemDto>.Success(_mapper.Map<InspectionItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Inspection item {id} was not found.");
        }

        public async Task<List<InspectionItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<InspectionItemDto>>(await _repository.GetAll(ct));
        public async Task<InspectionItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<InspectionItemDto>(entity);
        }

        public async Task<Result<InspectionItemDto>> UpdateAsync(ulong id, InspectionItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<InspectionItemDto>.Failure($"Inspection item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<InspectionItemDto>.Success(_mapper.Map<InspectionItemDto>(entity));
        }
    }

    public class InspectionExecutionService : IInspectionExecutionService
    {
        private readonly IRepository<Entities.InspectionExecution> _repo;
        private readonly IRepository<Entities.InspectionResult> _result;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InspectionExecutionService(
            IRepository<Entities.InspectionExecution> repo,
            IRepository<Entities.InspectionResult> result,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _result = result;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<InspectionExecutionDto>> CreateAsync(InspectionExecutionRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.ExecutionNo == request.ExecutionNo, ct);
            if (existing is not null)
            {
                return Result<InspectionExecutionDto>.Failure($"Inspection execution '{request.ExecutionNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.InspectionExecution>(request);
            var result = request.InspectionResults ?? new List<InspectionResultRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _result.AddRange(result.Select(r => ToResultEntity(r, entity.Id)).ToList(), token);

                return Result<InspectionExecutionDto>.Success(_mapper.Map<InspectionExecutionDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Inspection execution {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _result.Where(p => p.InspectionExecutionId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _result.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<InspectionExecutionDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<InspectionExecutionDto>>(await _repo.GetAll(ct));

        public async Task<InspectionExecutionDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<InspectionExecutionDto>(entity);
        }

        public async Task<Result<InspectionExecutionDto>> UpdateAsync(ulong id, InspectionExecutionRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<InspectionExecutionDto>.Failure($"Inspection execution {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.ExecutionNo == request.ExecutionNo, ct);
            if (existing is not null)
            {
                return Result<InspectionExecutionDto>.Failure($"Inspection execution '{request.ExecutionNo}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _result.Where(l => l.InspectionExecutionId == id, ct);
            var items = request.InspectionResults;
            var keptItemIds = (items ?? new List<InspectionResultRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptItemIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<InspectionExecutionDto>.Failure($"Result(s) {string.Join(", ", foreignItems)} do not belong to Inspection execution {id}.");
            }


            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _result.DeleteRange(storedItems.Where(s => !keptItemIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _result.Update(target, token);
                    }

                    await _result.AddRange(items.Where(l => l.Id == 0).Select(line => ToResultEntity(line, id)).ToList(), token);
                }

                return Result<InspectionExecutionDto>.Success(_mapper.Map<InspectionExecutionDto>(entity));
            }, ct);
        }
        private Entities.InspectionResult ToResultEntity(InspectionResultRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.InspectionResult>(line);
            entity.InspectionExecutionId = id;
            return entity;
        }
    }

    public class InspectionResultService : IInspectionResultService
    {
        private readonly IRepository<Entities.InspectionResult> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InspectionResultService(
            IRepository<Entities.InspectionResult> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<InspectionResultDto>> CreateAsync(InspectionResultRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.InspectionResult>(request);
            await _repository.Add(entity, ct);
            return Result<InspectionResultDto>.Success(_mapper.Map<InspectionResultDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Inspection result {id} was not found.");
        }

        public async Task<List<InspectionResultDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<InspectionResultDto>>(await _repository.GetAll(ct));
        public async Task<InspectionResultDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<InspectionResultDto>(entity);
        }

        public async Task<Result<InspectionResultDto>> UpdateAsync(ulong id, InspectionResultRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<InspectionResultDto>.Failure($"Inspection result {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<InspectionResultDto>.Success(_mapper.Map<InspectionResultDto>(entity));
        }
    }
}
