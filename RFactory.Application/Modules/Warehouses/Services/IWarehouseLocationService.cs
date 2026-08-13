using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Warehouses.Services;

public interface IWarehouseLocationService
{
    Task<List<WarehouseLocationDto>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseLocationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<WarehouseLocationDto>> CreateAsync(CreateWarehouseLocationRequest request, CancellationToken ct = default);
    Task<Result<WarehouseLocationDto>> UpdateAsync(ulong id, UpdateWarehouseLocationRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
