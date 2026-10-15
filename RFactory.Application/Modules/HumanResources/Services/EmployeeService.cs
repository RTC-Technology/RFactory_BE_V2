using AutoMapper;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.HumanResources.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IRepository<Employee> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken ct = default)
        {
            var employees = await _repository.GetAll(ct);
            return _mapper.Map<List<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var employee = await _repository.GetById(id, ct);
            return employee is null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(e => e.EmployeeCode == request.EmployeeCode, ct);
            if (existing is not null)
            {
                return Result<EmployeeDto>.Failure($"Employee code '{request.EmployeeCode}' already exists.");
            }

            var entity = _mapper.Map<Employee>(request);
            await _repository.Add(entity, ct);
            return Result<EmployeeDto>.Success(_mapper.Map<EmployeeDto>(entity));
        }

        public async Task<Result<EmployeeDto>> UpdateAsync(ulong id, UpdateEmployeeRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<EmployeeDto>.Failure($"Employee {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<EmployeeDto>.Success(_mapper.Map<EmployeeDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Employee {id} was not found.");
        }
    }
}