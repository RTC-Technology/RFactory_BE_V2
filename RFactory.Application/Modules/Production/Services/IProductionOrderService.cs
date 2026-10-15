using System.Collections.Generic;
using System.Threading.Tasks;
using RFactory.Application.Modules.Production.DTOs;

namespace RFactory.Application.Modules.Production.Services;

public interface IProductionOrderService
{
    Task<List<ProductionOrderDto>> GetAllAsync(ProductionOrderFilterDto? filter = null);
    Task<ProductionOrderDto?> GetByIdAsync(ulong id);
    Task<ProductionOrderDto> CreateAsync(ProductionOrderCreateRequest request, string currentUser);
    Task<ProductionOrderDto> UpdateAsync(ulong id, ProductionOrderUpdateRequest request, string currentUser);
    Task<bool> DeleteAsync(ulong id, string currentUser);
    Task<ProductionOrderDto> ChangeStatusAsync(ulong id, int newStatus, string currentUser, string? remark = null);
    Task<ProductionOrderDto> RecordProgressAsync(ulong id, ProductionOrderProgressRequest request, string currentUser);

    /// <summary>
    /// Dự tính chi tiết NVL theo BOM và số lượng sản xuất ("SL theo định mức = Định mức x SL sản xuất / SL quy định BOM")
    /// </summary>
    Task<List<ProductionOrderMaterialDto>> PreviewBomMaterialsAsync(ulong bomId, decimal plannedQuantity);

    /// <summary>
    /// Dự tính danh sách công đoạn theo quy trình sản xuất (Routing)
    /// </summary>
    Task<List<ProductionOrderOperationDto>> PreviewRoutingOperationsAsync(ulong routingId);

    /// <summary>
    /// Lấy lại chi tiết NVL theo BOM ban đầu cho lệnh sản xuất ("Lấy lại theo BOM")
    /// </summary>
    Task<ProductionOrderDto> ResetMaterialsToBomAsync(ulong orderId, string currentUser);

    /// <summary>
    /// Nhân bản lệnh sản xuất (Duplicate)
    /// </summary>
    Task<ProductionOrderDto> DuplicateAsync(ulong id, string currentUser);
}
