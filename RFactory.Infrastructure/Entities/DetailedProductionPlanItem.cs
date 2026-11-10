#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Nhu cầu sản phẩm của Kế hoạch sản xuất chi tiết
/// </summary>
public partial class DetailedProductionPlanItem
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public ulong PlanId { get; set; }
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public ulong? UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DailyCapacity { get; set; } = 100;
    public string SalesOrderNo { get; set; }
    public int Sequence { get; set; } = 1;
    public string Remark { get; set; }
}
