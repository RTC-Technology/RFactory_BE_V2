#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Chi tiết nguyên vật liệu theo lệnh sản xuất (Production Order Material)
/// </summary>
public partial class ProductionOrderMaterial
{
    public ulong Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public ulong ProductionOrderId { get; set; }

    public ulong ProductId { get; set; }

    public string ProductCode { get; set; }

    public string ProductName { get; set; }

    public ulong? UnitId { get; set; }

    public string UnitName { get; set; }

    /// <summary>
    /// Định mức theo 1 đơn vị thành phẩm
    /// </summary>
    public decimal NormQuantity { get; set; }

    /// <summary>
    /// Số lượng theo định mức (= NormQuantity * PlannedQuantity)
    /// </summary>
    public decimal RequiredQuantity { get; set; }

    /// <summary>
    /// Số lượng đã xuất kho
    /// </summary>
    public decimal IssuedQuantity { get; set; }

    /// <summary>
    /// Tồn kho khả dụng
    /// </summary>
    public decimal CurrentStock { get; set; }

    public ulong? WarehouseId { get; set; }

    public string WarehouseName { get; set; }

    /// <summary>
    /// Là vật tư thay thế
    /// </summary>
    public bool IsSubstituted { get; set; }

    /// <summary>
    /// ID vật tư ban đầu nếu thay thế
    /// </summary>
    public ulong? OriginalProductId { get; set; }

    public string Remark { get; set; }
}
