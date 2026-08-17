using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public class SupplierService:ISupplierService
    {
        private readonly IRepository<Supplier> _repository;
        private readonly IMapper _mapper;

        public SupplierService(IRepository<Supplier> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SupplierDto>> GetAllAsync(CancellationToken ct = default)
        {
            var areas = await _repository.GetAll(ct);
            return _mapper.Map<List<SupplierDto>>(areas);
        }

        public async Task<SupplierDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var area = await _repository.GetById(id, ct);
            return area is null ? null : _mapper.Map<SupplierDto>(area);
        }

        public async Task<Result<SupplierDto>> CreateAsync(CreateSupplierRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(a => a.SupplierCode == request.SupplierCode, ct);
            if (existing is not null)
            {
                return Result<SupplierDto>.Failure($"Supplier code '{request.SupplierCode}' already exists.");
            }

            var entity = _mapper.Map<Supplier>(request);
            await _repository.Add(entity, ct);
            return Result<SupplierDto>.Success(_mapper.Map<SupplierDto>(entity));
        }

        public async Task<Result<SupplierDto>> UpdateAsync(ulong id, UpdateSupplierRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SupplierDto>.Failure($"Supplier {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SupplierDto>.Success(_mapper.Map<SupplierDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Supplier {id} was not found.");
        }
    }
}
