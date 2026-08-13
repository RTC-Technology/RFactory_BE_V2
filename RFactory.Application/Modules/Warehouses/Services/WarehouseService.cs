using AutoMapper;
using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Warehouses.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IRepository<Entities.Warehouse> _repository;
    private readonly IMapper _mapper;

    public WarehouseService(IRepository<Entities.Warehouse> repository, IMapper mapper)
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

    public async Task<Result<WarehouseDto>> CreateAsync(CreateWarehouseRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(w => w.WarehouseCode == request.WarehouseCode, ct);
        if (existing is not null)
        {
            return Result<WarehouseDto>.Failure($"Warehouse code '{request.WarehouseCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.Warehouse>(request);
        await _repository.Add(entity, ct);
        return Result<WarehouseDto>.Success(_mapper.Map<WarehouseDto>(entity));
    }

    public async Task<Result<WarehouseDto>> UpdateAsync(ulong id, UpdateWarehouseRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<WarehouseDto>.Failure($"Warehouse {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            w => w.Id != id && w.WarehouseCode == request.WarehouseCode, ct);
        if (existing is not null)
        {
            return Result<WarehouseDto>.Failure($"Warehouse code '{request.WarehouseCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<WarehouseDto>.Success(_mapper.Map<WarehouseDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Warehouse {id} was not found.");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}