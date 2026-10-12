using AutoMapper;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Organizations.Services
{
    public class WorkCenterService : IWorkCenterService
    {
        private readonly IRepository<Entities.WorkCenter> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkCenterService(
            IRepository<Entities.WorkCenter> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<WorkCenterDto>> CreateAsync(WorkCenterRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.WorkCenterCode == request.WorkCenterCode, ct);
            if (existing is not null)
            {
                return Result<WorkCenterDto>.Failure($"WorkCenter code '{request.WorkCenterCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.WorkCenter>(request);
            await _repository.Add(entity, ct);
            return Result<WorkCenterDto>.Success(_mapper.Map<WorkCenterDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"WorkCenter {id} was not found.");
        }

        public async Task<List<WorkCenterDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<WorkCenterDto>>(await _repository.GetAll(ct));
        public async Task<WorkCenterDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<WorkCenterDto>(entity);
        }

        public async Task<Result<WorkCenterDto>> UpdateAsync(ulong id, WorkCenterRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<WorkCenterDto>.Failure($"WorkCenter {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.WorkCenterCode == request.WorkCenterCode, ct);
            if (existing is not null)
            {
                return Result<WorkCenterDto>.Failure($"WorkCenter code '{request.WorkCenterCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<WorkCenterDto>.Success(_mapper.Map<WorkCenterDto>(entity));
        }
    }
}
