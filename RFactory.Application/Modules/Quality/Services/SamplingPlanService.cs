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
    public class SamplingPlanService: ISamplingPlanService
    {
        private readonly IRepository<Entities.SamplingPlan> _repo;
        private readonly IRepository<Entities.SamplingPlanRule> _rule;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SamplingPlanService(
            IRepository<Entities.SamplingPlan> repo,
            IRepository<Entities.SamplingPlanRule> rule,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _rule = rule;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<SamplingPlanDto>> CreateAsync(SamplingPlanRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.SamplingPlanCode == request.SamplingPlanCode, ct);
            if (existing is not null)
            {
                return Result<SamplingPlanDto>.Failure($"Sampling plan '{request.SamplingPlanCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.SamplingPlan>(request);
            var rules = request.SamplingPlanRules ?? new List<SamplingPlanRuleRequest>();
            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _rule.AddRange(rules.Select(rule => ToRuleEntity(rule, entity.Id)).ToList(), token);

                return Result<SamplingPlanDto>.Success(_mapper.Map<SamplingPlanDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Sampling plan {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _rule.Where(p => p.SamplingPlanId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _rule.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<SamplingPlanDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<SamplingPlanDto>>(await _repo.GetAll(ct));

        public async Task<SamplingPlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SamplingPlanDto>(entity);
        }

        public async Task<Result<SamplingPlanDto>> UpdateAsync(ulong id, SamplingPlanRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<SamplingPlanDto>.Failure($"Sampling plan {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.SamplingPlanCode == request.SamplingPlanCode, ct);
            if (existing is not null)
            {
                return Result<SamplingPlanDto>.Failure($"Sampling plan '{request.SamplingPlanCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _rule.Where(l => l.SamplingPlanId == id, ct);
            var rules = request.SamplingPlanRules;
            var keptIds = (rules ?? new List<SamplingPlanRuleRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<SamplingPlanDto>.Failure($"Rule(s) {string.Join(", ", foreignItems)} do not belong to Sampling plan {id}.");
            }


            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (rules is not null)
                {
                    await _rule.DeleteRange(storedItems.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in rules.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _rule.Update(target, token);
                    }

                    await _rule.AddRange(rules.Where(l => l.Id == 0).Select(line => ToRuleEntity(line, id)).ToList(), token);
                }

                return Result<SamplingPlanDto>.Success(_mapper.Map<SamplingPlanDto>(entity));
            }, ct);
        }

        private Entities.SamplingPlanRule ToRuleEntity(SamplingPlanRuleRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.SamplingPlanRule>(line);
            entity.SamplingPlanId = id;
            return entity;
        }
    }

    public class SamplingPlanRuleService : ISamplingPlanRuleService
    {
        private readonly IRepository<Entities.SamplingPlanRule> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SamplingPlanRuleService(
            IRepository<Entities.SamplingPlanRule> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<SamplingPlanRuleDto>> CreateAsync(SamplingPlanRuleRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.SamplingPlanRule>(request);
            await _repository.Add(entity, ct);
            return Result<SamplingPlanRuleDto>.Success(_mapper.Map<SamplingPlanRuleDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Sampling plan rule {id} was not found.");
        }

        public async Task<List<SamplingPlanRuleDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<SamplingPlanRuleDto>>(await _repository.GetAll(ct));
        public async Task<SamplingPlanRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SamplingPlanRuleDto>(entity);
        }

        public async Task<Result<SamplingPlanRuleDto>> UpdateAsync(ulong id, SamplingPlanRuleRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SamplingPlanRuleDto>.Failure($"Sampling plan rule {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SamplingPlanRuleDto>.Success(_mapper.Map<SamplingPlanRuleDto>(entity));
        }
    }
}
