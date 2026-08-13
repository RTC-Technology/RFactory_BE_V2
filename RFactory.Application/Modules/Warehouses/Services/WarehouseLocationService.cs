using AutoMapper;
using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Warehouses.Services;

public class WarehouseLocationService : IWarehouseLocationService
{
    private readonly IRepository<Entities.WarehouseLocation> _repository;
    private readonly IMapper _mapper;

    public WarehouseLocationService(IRepository<Entities.WarehouseLocation> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<WarehouseLocationDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<WarehouseLocationDto>>(await _repository.GetAll(ct));

    public async Task<WarehouseLocationDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<WarehouseLocationDto>(entity);
    }

    public async Task<Result<WarehouseLocationDto>> CreateAsync(CreateWarehouseLocationRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(l => l.WarehouseLocationCode == request.WarehouseLocationCode, ct);
        if (existing is not null)
        {
            return Result<WarehouseLocationDto>.Failure($"Warehouse location code '{request.WarehouseLocationCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.WarehouseLocation>(request);
        await _repository.Add(entity, ct);
        return Result<WarehouseLocationDto>.Success(_mapper.Map<WarehouseLocationDto>(entity));
    }

    public async Task<Result<WarehouseLocationDto>> UpdateAsync(ulong id, UpdateWarehouseLocationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<WarehouseLocationDto>.Failure($"Warehouse location {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            l => l.Id != id && l.WarehouseLocationCode == request.WarehouseLocationCode, ct);
        if (existing is not null)
        {
            return Result<WarehouseLocationDto>.Failure($"Warehouse location code '{request.WarehouseLocationCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<WarehouseLocationDto>.Success(_mapper.Map<WarehouseLocationDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Warehouse location {id} was not found.");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}