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
    public class WorkshopService : IWorkshopService
    {
        private readonly IRepository<Entities.Workshop> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkshopService(
            IRepository<Entities.Workshop> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<WorkshopDto>> CreateAsync(WorkshopRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.WorkshopCode == request.WorkshopCode, ct);
            if (existing is not null)
            {
                return Result<WorkshopDto>.Failure($"Workshop code '{request.WorkshopCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Workshop>(request);
            await _repository.Add(entity, ct);
            return Result<WorkshopDto>.Success(_mapper.Map<WorkshopDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Workshop {id} was not found.");
        }

        public async Task<List<WorkshopDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<WorkshopDto>>(await _repository.GetAll(ct));
        public async Task<WorkshopDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<WorkshopDto>(entity);
        }

        public async Task<Result<WorkshopDto>> UpdateAsync(ulong id, WorkshopRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<WorkshopDto>.Failure($"Workshop {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.WorkshopCode == request.WorkshopCode, ct);
            if (existing is not null)
            {
                return Result<WorkshopDto>.Failure($"Workshop code '{request.WorkshopCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<WorkshopDto>.Success(_mapper.Map<WorkshopDto>(entity));
        }
    }
}
