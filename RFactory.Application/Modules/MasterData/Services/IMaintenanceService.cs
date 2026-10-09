using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface IMaintenanceTypeService
    {
        Task<List<MaintenanceTypeDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenanceTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenanceTypeDto>> CreateAsync(MaintenanceTypeRequest request, CancellationToken ct = default);
        Task<Result<MaintenanceTypeDto>> UpdateAsync(ulong id, MaintenanceTypeRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IMaintenanceChecklistService
    {
        Task<List<MaintenanceChecklistDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenanceChecklistDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenanceChecklistDto>> CreateAsync(MaintenanceChecklistRequest request, CancellationToken ct = default);
        Task<Result<MaintenanceChecklistDto>> UpdateAsync(ulong id, MaintenanceChecklistRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IMaintenanceChecklistItemService
    {
        Task<List<MaintenanceChecklistItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenanceChecklistItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenanceChecklistItemDto>> CreateAsync(MaintenanceChecklistItemRequest request, CancellationToken ct = default);
        Task<Result<MaintenanceChecklistItemDto>> UpdateAsync(ulong id, MaintenanceChecklistItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IMaintenancePlanService
    {
        Task<List<MaintenancePlanDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenancePlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenancePlanDto>> CreateAsync(MaintenancePlanRequest request, CancellationToken ct = default);
        Task<Result<MaintenancePlanDto>> UpdateAsync(ulong id, MaintenancePlanRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }


    public interface IMaintenanceOrderService
    {
        Task<List<MaintenanceOrderDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenanceOrderDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenanceOrderDto>> CreateAsync(MaintenanceOrderRequest request, CancellationToken ct = default);
        Task<Result<MaintenanceOrderDto>> UpdateAsync(ulong id, MaintenanceOrderRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IMaintenanceOrderChecklistService
    {
        Task<List<MaintenanceOrderChecklistDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenanceOrderChecklistDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenanceOrderChecklistDto>> CreateAsync(MaintenanceOrderChecklistRequest request, CancellationToken ct = default);
        Task<Result<MaintenanceOrderChecklistDto>> UpdateAsync(ulong id, MaintenanceOrderChecklistRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IMaintenanceOrderChecklistItemService
    {
        Task<List<MaintenanceOrderChecklistItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<MaintenanceOrderChecklistItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<MaintenanceOrderChecklistItemDto>> CreateAsync(MaintenanceOrderChecklistItemRequest request, CancellationToken ct = default);
        Task<Result<MaintenanceOrderChecklistItemDto>> UpdateAsync(ulong id, MaintenanceOrderChecklistItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
