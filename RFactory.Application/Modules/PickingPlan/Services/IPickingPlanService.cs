using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.PickingPlan.Services
{
    public interface IPickingPlanService
    {
        Task<List<PickingPlanDto>> GetAllAsync(CancellationToken ct = default);
        Task<PickingPlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PickingPlanDto>> CreateAsync(PickingPlanRequest request, CancellationToken ct = default);
        Task<Result<PickingPlanDto>> UpdateAsync(ulong id, PickingPlanRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IPickingPlanItemService
    {
        Task<List<PickingPlanItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<PickingPlanItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PickingPlanItemDto>> CreateAsync(PickingPlanItemRequest request, CancellationToken ct = default);
        Task<Result<PickingPlanItemDto>> UpdateAsync(ulong id, PickingPlanItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IPickingPlanItemSourceService
    {
        Task<List<PickingPlanItemSourceDto>> GetAllAsync(CancellationToken ct = default);
        Task<PickingPlanItemSourceDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PickingPlanItemSourceDto>> CreateAsync(PickingPlanItemSourceRequest request, CancellationToken ct = default);
        Task<Result<PickingPlanItemSourceDto>> UpdateAsync(ulong id, PickingPlanItemSourceRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IPickingPlanSourceService
    {
        Task<List<PickingPlanSourceDto>> GetAllAsync(CancellationToken ct = default);
        Task<PickingPlanSourceDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PickingPlanSourceDto>> CreateAsync(PickingPlanSourceRequest request, CancellationToken ct = default);
        Task<Result<PickingPlanSourceDto>> UpdateAsync(ulong id, PickingPlanSourceRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IPickingTicketService
    {
        Task<List<PickingTicketDto>> GetAllAsync(CancellationToken ct = default);
        Task<PickingTicketDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PickingTicketDto>> CreateAsync(PickingTicketRequest request, CancellationToken ct = default);
        Task<Result<PickingTicketDto>> UpdateAsync(ulong id, PickingTicketRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IPickingTicketItemService
    {
        Task<List<PickingTicketItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<PickingTicketItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PickingTicketItemDto>> CreateAsync(PickingTicketItemRequest request, CancellationToken ct = default);
        Task<Result<PickingTicketItemDto>> UpdateAsync(ulong id, PickingTicketItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
