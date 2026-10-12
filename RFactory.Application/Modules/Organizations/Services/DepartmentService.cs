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
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Entities.Department> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(
            IRepository<Entities.Department> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDto>> CreateAsync(DepartmentRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.DepartmentCode == request.DepartmentCode, ct);
            if (existing is not null)
            {
                return Result<DepartmentDto>.Failure($"Department code '{request.DepartmentCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Department>(request);
            await _repository.Add(entity, ct);
            return Result<DepartmentDto>.Success(_mapper.Map<DepartmentDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Department {id} was not found.");
        }

        public async Task<List<DepartmentDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DepartmentDto>>(await _repository.GetAll(ct));
        public async Task<DepartmentDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DepartmentDto>(entity);
        }

        public async Task<Result<DepartmentDto>> UpdateAsync(ulong id, DepartmentRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DepartmentDto>.Failure($"Department {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.DepartmentCode == request.DepartmentCode, ct);
            if (existing is not null)
            {
                return Result<DepartmentDto>.Failure($"Department code '{request.DepartmentCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DepartmentDto>.Success(_mapper.Map<DepartmentDto>(entity));
        }
    }
}
