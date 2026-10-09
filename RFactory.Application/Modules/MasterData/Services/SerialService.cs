using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
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
    public class SerialService:ISerialService
    {
        private readonly IRepository<Entities.Serial> _repository;
        private readonly IMapper _mapper;

        public SerialService(
            IRepository<Entities.Serial> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SerialDto>> CreateAsync(SerialRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.SerialNo == request.SerialNo, ct);
            if (existing is not null)
            {
                return Result<SerialDto>.Failure($"Serial No '{request.SerialNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.Serial>(request);
            await _repository.Add(entity, ct);
            return Result<SerialDto>.Success(_mapper.Map<SerialDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Serial {id} was not found.");
        }

        public async Task<List<SerialDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<SerialDto>>(await _repository.GetAll(ct));
        public async Task<SerialDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SerialDto>(entity);
        }

        public async Task<Result<SerialDto>> UpdateAsync(ulong id, SerialRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SerialDto>.Failure($"Serial {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.SerialNo == request.SerialNo, ct);
            if (existing is not null)
            {
                return Result<SerialDto>.Failure($"Serial No '{request.SerialNo}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SerialDto>.Success(_mapper.Map<SerialDto>(entity));
        }
    }

    public class SerialRuleService : ISerialRuleService
    {
        private readonly IRepository<Entities.SerialRule> _repository;
        private readonly IMapper _mapper;

        public SerialRuleService(
            IRepository<Entities.SerialRule> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SerialRuleDto>> CreateAsync(SerialRuleRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.RuleCode == request.RuleCode, ct);
            if (existing is not null)
            {
                return Result<SerialRuleDto>.Failure($"Serial Rule '{request.RuleCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.SerialRule>(request);
            await _repository.Add(entity, ct);
            return Result<SerialRuleDto>.Success(_mapper.Map<SerialRuleDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Serial Rule {id} was not found.");
        }

        public async Task<List<SerialRuleDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<SerialRuleDto>>(await _repository.GetAll(ct));
        public async Task<SerialRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SerialRuleDto>(entity);
        }

        public async Task<Result<SerialRuleDto>> UpdateAsync(ulong id, SerialRuleRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SerialRuleDto>.Failure($"Serial Rule {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.RuleCode == request.RuleCode, ct);
            if (existing is not null)
            {
                return Result<SerialRuleDto>.Failure($"Serial Rule '{request.RuleCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SerialRuleDto>.Success(_mapper.Map<SerialRuleDto>(entity));
        }
    }

    public class SerialRuleSequenceService : ISerialRuleSequenceService
    {
        private readonly IRepository<Entities.SerialRuleSequence> _repository;
        private readonly IMapper _mapper;

        public SerialRuleSequenceService(
            IRepository<Entities.SerialRuleSequence> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SerialRuleSequenceDto>> CreateAsync(SerialRuleSequenceRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.SequencePeriod == request.SequencePeriod, ct);
            if (existing is not null)
            {
                return Result<SerialRuleSequenceDto>.Failure($"Serial Rule Sequence '{request.SequencePeriod}' already exists.");
            }

            var entity = _mapper.Map<Entities.SerialRuleSequence>(request);
            await _repository.Add(entity, ct);
            return Result<SerialRuleSequenceDto>.Success(_mapper.Map<SerialRuleSequenceDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Serial Rule Sequence {id} was not found.");
        }

        public async Task<List<SerialRuleSequenceDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<SerialRuleSequenceDto>>(await _repository.GetAll(ct));
        public async Task<SerialRuleSequenceDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SerialRuleSequenceDto>(entity);
        }

        public async Task<Result<SerialRuleSequenceDto>> UpdateAsync(ulong id, SerialRuleSequenceRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SerialRuleSequenceDto>.Failure($"Serial Rule Sequence {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.SequencePeriod == request.SequencePeriod, ct);
            if (existing is not null)
            {
                return Result<SerialRuleSequenceDto>.Failure($"Serial Rule Sequence '{request.SequencePeriod}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SerialRuleSequenceDto>.Success(_mapper.Map<SerialRuleSequenceDto>(entity));
        }
    }


}
