using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Production.Services
{
    public interface ISalesOrderService
    {
        Task<List<SalesOrderDto>> GetAllAsync(CancellationToken ct = default);
        Task<SalesOrderDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SalesOrderDto>> CreateAsync(SalesOrderRequest request, CancellationToken ct = default);
        Task<Result<SalesOrderDto>> UpdateAsync(ulong id, SalesOrderRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ISalesOrderItemService
    {
        Task<List<SalesOrderItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<SalesOrderItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SalesOrderItemDto>> CreateAsync(SalesOrderItemRequest request, CancellationToken ct = default);
        Task<Result<SalesOrderItemDto>> UpdateAsync(ulong id, SalesOrderItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ISalesOrderPaymentScheduleService
    {
        Task<List<SalesOrderPaymentScheduleDto>> GetAllAsync(CancellationToken ct = default);
        Task<SalesOrderPaymentScheduleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SalesOrderPaymentScheduleDto>> CreateAsync(SalesOrderPaymentScheduleRequest request, CancellationToken ct = default);
        Task<Result<SalesOrderPaymentScheduleDto>> UpdateAsync(ulong id, SalesOrderPaymentScheduleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
