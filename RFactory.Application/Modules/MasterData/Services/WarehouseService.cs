using AutoMapper;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
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
    public class WarehouseService:IWarehouseService
    {
        private readonly IRepository<Infrastructure.Entities.Warehouse> _repository;
        private readonly IMapper _mapper;

        public WarehouseService(IRepository<Infrastructure.Entities.Warehouse> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<WarehouseDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<WarehouseDto>>(await _repository.GetAll(ct));

        public async Task<WarehouseDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<WarehouseDto>(entity);
        }
    }

    public class WarehouseLocationService : IWarehouseLocationService
    {
        private readonly IRepository<Infrastructure.Entities.WarehouseLocation> _repository;
        private readonly IMapper _mapper;

        public WarehouseLocationService(IRepository<Infrastructure.Entities.WarehouseLocation> repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Location {id} was not found.");
        }

        public async Task<List<WarehouseLocationDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<WarehouseLocationDto>>(await _repository.GetAll(ct));

        public async Task<WarehouseLocationDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<WarehouseLocationDto>(entity);
        }
    }
}
