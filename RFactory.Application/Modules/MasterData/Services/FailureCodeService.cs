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
    public class FailureCodeService:IFailureCodeService
    {
        private readonly IRepository<Entities.FailureCode> _repository;
        private readonly IMapper _mapper;

        public FailureCodeService(
            IRepository<Entities.FailureCode> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<FailureCodeDto>> CreateAsync(FailureCodeRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.Code == request.Code, ct);
            if (existing is not null)
            {
                return Result<FailureCodeDto>.Failure($"Failure code '{request.Code}' already exists.");
            }

            var entity = _mapper.Map<Entities.FailureCode>(request);
            await _repository.Add(entity, ct);
            return Result<FailureCodeDto>.Success(_mapper.Map<FailureCodeDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Failure code {id} was not found.");
        }

        public async Task<List<FailureCodeDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<FailureCodeDto>>(await _repository.GetAll(ct));
        public async Task<FailureCodeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<FailureCodeDto>(entity);
        }

        public async Task<Result<FailureCodeDto>> UpdateAsync(ulong id, FailureCodeRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<FailureCodeDto>.Failure($"Failure code {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.Code == request.Code, ct);
            if (existing is not null)
            {
                return Result<FailureCodeDto>.Failure($"Failure code '{request.Code}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<FailureCodeDto>.Success(_mapper.Map<FailureCodeDto>(entity));
        }
    }
    public class FailureGroupService:IFailureGroupService
    {
        private readonly IRepository<Entities.FailureGroup> _repository;
        private readonly IMapper _mapper;

        public FailureGroupService(
            IRepository<Entities.FailureGroup> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<FailureGroupDto>> CreateAsync(FailureGroupRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.GroupCode == request.GroupCode, ct);
            if (existing is not null)
            {
                return Result<FailureGroupDto>.Failure($"Failure group '{request.GroupCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.FailureGroup>(request);
            await _repository.Add(entity, ct);
            return Result<FailureGroupDto>.Success(_mapper.Map<FailureGroupDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Failure group {id} was not found.");
        }

        public async Task<List<FailureGroupDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<FailureGroupDto>>(await _repository.GetAll(ct));
        public async Task<FailureGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<FailureGroupDto>(entity);
        }

        public async Task<Result<FailureGroupDto>> UpdateAsync(ulong id, FailureGroupRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<FailureGroupDto>.Failure($"Failure group {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.GroupCode == request.GroupCode, ct);
            if (existing is not null)
            {
                return Result<FailureGroupDto>.Failure($"Failure group '{request.GroupCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<FailureGroupDto>.Success(_mapper.Map<FailureGroupDto>(entity));
        }
    }
}
