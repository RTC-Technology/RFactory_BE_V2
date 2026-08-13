using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Warehouses.Services;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<WarehouseDto>> CreateAsync(CreateWarehouseRequest request, CancellationToken ct = default);
    Task<Result<WarehouseDto>> UpdateAsync(ulong id, UpdateWarehouseRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
