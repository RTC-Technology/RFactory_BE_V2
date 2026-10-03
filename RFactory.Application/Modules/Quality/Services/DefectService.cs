using AutoMapper;
using RFactory.Application.Modules.Organizations.DTOs;
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
    public class DefectService:IDefectService
    {
        private readonly IRepository<Entities.Defect> _repository;
        private readonly IMapper _mapper;

        public DefectService(
            IRepository<Entities.Defect> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<DefectDto>> CreateAsync(DefectRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.DefectCode == request.DefectCode, ct);
            if (existing is not null)
            {
                return Result<DefectDto>.Failure($"Defect code '{request.DefectCode }' already exists.");
            }

            var entity = _mapper.Map<Entities.Defect>(request);
            await _repository.Add(entity, ct);
            return Result<DefectDto>.Success(_mapper.Map<DefectDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Defect {id} was not found.");
        }

        public async Task<List<DefectDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DefectDto>>(await _repository.GetAll(ct));
        public async Task<DefectDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DefectDto>(entity);
        }

        public async Task<Result<DefectDto>> UpdateAsync(ulong id, DefectRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DefectDto>.Failure($"Defect {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.DefectCode == request.DefectCode, ct);
            if (existing is not null)
            {
                return Result<DefectDto>.Failure($"Defect code '{request.DefectCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DefectDto>.Success(_mapper.Map<DefectDto>(entity));
        }
    }

    public class DefectGroupService : IDefectGroupService
    {
        private readonly IRepository<Entities.DefectGroup> _repository;
        private readonly IRepository<Entities.Defect> _defect;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DefectGroupService(
            IRepository<Entities.DefectGroup> repository,
            IRepository<Entities.Defect> defect,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _defect = defect;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DefectGroupDto>> CreateAsync(DefectGroupRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.GroupCode == request.GroupCode, ct);
            if (existing is not null)
            {
                return Result<DefectGroupDto>.Failure($"Defect group code '{request.GroupCode }' already exists.");
            }

            var entity = _mapper.Map<Entities.DefectGroup>(request);
            await _repository.Add(entity, ct);
            return Result<DefectGroupDto>.Success(_mapper.Map<DefectGroupDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Defect group {id} was not found.");
            }

            var defects = await _defect.Where(p => p.DefectGroupId == id, ct);
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _defect.DeleteRange(defects, token);
                await _repository.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<DefectGroupDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DefectGroupDto>>(await _repository.GetAll(ct));
        public async Task<DefectGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DefectGroupDto>(entity);
        }

        public async Task<Result<DefectGroupDto>> UpdateAsync(ulong id, DefectGroupRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DefectGroupDto>.Failure($"Defect group {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.GroupCode == request.GroupCode, ct);
            if (existing is not null)
            {
                return Result<DefectGroupDto>.Failure($"Defect group code '{request.GroupCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DefectGroupDto>.Success(_mapper.Map<DefectGroupDto>(entity));
        }
    }
}
