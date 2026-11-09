#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Lịch phân bổ sản xuất theo ngày/ca của Kế hoạch sản xuất chi tiết
/// </summary>
public partial class DetailedProductionPlanSchedule
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public ulong PlanId { get; set; }
    public ulong PlanItemId { get; set; }
    public ulong ProductId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public ulong? ShiftId { get; set; }
    public string ShiftName { get; set; }
    public decimal Capacity { get; set; }
    public decimal PlannedQuantity { get; set; }
    public bool IsDayOff { get; set; }
    public bool IsOvertime { get; set; }
    public ulong? ProductionOrderId { get; set; }
    public string ProductionOrderNo { get; set; }
    public string Remark { get; set; }
}
