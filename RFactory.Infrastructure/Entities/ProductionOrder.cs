#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Lệnh sản xuất (Production Order)
/// </summary>
public partial class ProductionOrder
{
    /// <summary>
    /// Primary Key
    /// </summary>
    public ulong Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    /// <summary>
    /// Mã lệnh sản xuất
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// Loại lệnh sản xuất: 1=MTS (Kế hoạch tổng thể), 2=MTO (Đơn hàng), 3=MRP (Kế hoạch chi tiết), 4=Other (Khác/Trực tiếp)
    /// </summary>
    public int OrderType { get; set; } = 4;

    /// <summary>
    /// Mô tả lệnh sản xuất
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// ID sản phẩm cần sản xuất
    /// </summary>
    public ulong ProductId { get; set; }

    /// <summary>
    /// ID đơn vị tính
    /// </summary>
    public ulong? UnitId { get; set; }

    /// <summary>
    /// ID định mức BOM
    /// </summary>
    public ulong? BomId { get; set; }

    /// <summary>
    /// ID quy trình công nghệ Routing
    /// </summary>
    public ulong? RoutingId { get; set; }

    /// <summary>
    /// ID nhà máy
    /// </summary>
    public ulong? FactoryId { get; set; }

    /// <summary>
    /// ID dây chuyền sản xuất
    /// </summary>
    public ulong? LineId { get; set; }

    /// <summary>
    /// ID tổ / trung tâm gia công
    /// </summary>
    public ulong? WorkCenterId { get; set; }

    /// <summary>
    /// ID khách hàng
    /// </summary>
    public ulong? CustomerId { get; set; }

    /// <summary>
    /// Số lượng kế hoạch
    /// </summary>
    public decimal PlannedQuantity { get; set; }

    /// <summary>
    /// Số lượng hoàn thành
    /// </summary>
    public decimal CompletedQuantity { get; set; }

    /// <summary>
    /// Số lượng phế phẩm
    /// </summary>
    public decimal ScrapQuantity { get; set; }

    /// <summary>
    /// Ngày bắt đầu kế hoạch
    /// </summary>
    public DateTime? PlannedStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc kế hoạch
    /// </summary>
    public DateTime? PlannedEndDate { get; set; }

    /// <summary>
    /// Ngày bắt đầu thực tế
    /// </summary>
    public DateTime? ActualStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc thực tế
    /// </summary>
    public DateTime? ActualEndDate { get; set; }

    /// <summary>
    /// Độ ưu tiên: 1=Low, 2=Normal, 3=High, 4=Urgent
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Trạng thái: 1=Draft, 2=Released, 3=InProgress, 4=Completed, 5=Cancelled, 6=Closed
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Số lô sản xuất (Batch / Lot No)
    /// </summary>
    public string BatchNo { get; set; }

    /// <summary>
    /// Mã đơn hàng bán tham chiếu
    /// </summary>
    public string SalesOrderNo { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// Danh sách chi tiết nguyên vật liệu (BOM Materials)
    /// </summary>
    public virtual System.Collections.Generic.ICollection<ProductionOrderMaterial> Materials { get; set; } = new System.Collections.Generic.List<ProductionOrderMaterial>();

    /// <summary>
    /// Danh sách công đoạn sản xuất (Routing Operations)
    /// </summary>
    public virtual System.Collections.Generic.ICollection<ProductionOrderOperation> Operations { get; set; } = new System.Collections.Generic.List<ProductionOrderOperation>();
}
