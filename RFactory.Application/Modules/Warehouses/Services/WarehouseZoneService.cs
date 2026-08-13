using AutoMapper;
using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Warehouse.Services;

public interface IWarehouseZoneService
{
    Task<List<WarehouseZoneDto>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseZoneDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<WarehouseZoneDto>> CreateAsync(CreateWarehouseZoneRequest request, CancellationToken ct = default);
    Task<Result<WarehouseZoneDto>> UpdateAsync(ulong id, UpdateWarehouseZoneRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public class WarehouseZoneService : IWarehouseZoneService
{
    private readonly IRepository<Entities.WarehouseZone> _repository;
    private readonly IMapper _mapper;

    public WarehouseZoneService(IRepository<Entities.WarehouseZone> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<WarehouseZoneDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<WarehouseZoneDto>>(await _repository.GetAll(ct));

    public async Task<WarehouseZoneDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<WarehouseZoneDto>(entity);
    }

    public async Task<Result<WarehouseZoneDto>> CreateAsync(CreateWarehouseZoneRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(z => z.WarehouseZoneCode == request.WarehouseZoneCode, ct);
        if (existing is not null)
        {
            return Result<WarehouseZoneDto>.Failure($"Warehouse zone code '{request.WarehouseZoneCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.WarehouseZone>(request);
        await _repository.Add(entity, ct);
        return Result<WarehouseZoneDto>.Success(_mapper.Map<WarehouseZoneDto>(entity));
    }

    public async Task<Result<WarehouseZoneDto>> UpdateAsync(ulong id, UpdateWarehouseZoneRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<WarehouseZoneDto>.Failure($"Warehouse zone {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            z => z.Id != id && z.WarehouseZoneCode == request.WarehouseZoneCode, ct);
        if (existing is not null)
        {
            return Result<WarehouseZoneDto>.Failure($"Warehouse zone code '{request.WarehouseZoneCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<WarehouseZoneDto>.Success(_mapper.Map<WarehouseZoneDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Warehouse zone {id} was not found.");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}