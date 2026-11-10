#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Chi tiết sản phẩm kế hoạch sản xuất tổng thể
/// </summary>
public partial class MasterProductionPlanItem
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public ulong MasterPlanId { get; set; }
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public ulong? UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public decimal DailyCapacity { get; set; } = 100;
    public string Remark { get; set; }
}
