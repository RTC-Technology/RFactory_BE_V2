using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Warehouses.Services;

public interface IWarehouseZoneService
{
    Task<List<WarehouseZoneDto>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseZoneDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<WarehouseZoneDto>> CreateAsync(CreateWarehouseZoneRequest request, CancellationToken ct = default);
    Task<Result<WarehouseZoneDto>> UpdateAsync(ulong id, UpdateWarehouseZoneRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
