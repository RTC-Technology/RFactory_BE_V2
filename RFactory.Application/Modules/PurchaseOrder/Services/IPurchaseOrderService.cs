using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.PurchaseOrder.Services
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default);
        Task<PurchaseOrderDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PurchaseOrderDto>> CreateAsync(PurchaseOrderRequest request, CancellationToken ct = default);
        Task<Result<PurchaseOrderDto>> UpdateAsync(ulong id, PurchaseOrderRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IPurchaseOrderDetailService
    {
        Task<List<PurchaseOrderDetailDto>> GetAllAsync(CancellationToken ct = default);
        Task<PurchaseOrderDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PurchaseOrderDetailDto>> CreateAsync(PurchaseOrderDetailRequest request, CancellationToken ct = default);
        Task<Result<PurchaseOrderDetailDto>> UpdateAsync(ulong id, PurchaseOrderDetailRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IPurchaseOrderDeliveryScheduleService
    {
        Task<List<PurchaseOrderDeliveryScheduleDto>> GetAllAsync(CancellationToken ct = default);
        Task<PurchaseOrderDeliveryScheduleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PurchaseOrderDeliveryScheduleDto>> CreateAsync(PurchaseOrderDeliveryScheduleRequest request, CancellationToken ct = default);
        Task<Result<PurchaseOrderDeliveryScheduleDto>> UpdateAsync(ulong id, PurchaseOrderDeliveryScheduleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
