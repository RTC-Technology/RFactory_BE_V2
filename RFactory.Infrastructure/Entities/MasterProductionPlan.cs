#nullable disable
using System;
using System.Collections.Generic;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Kế hoạch sản xuất tổng thể (Master Production Plan)
/// </summary>
public partial class MasterProductionPlan
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public string PlanCode { get; set; }
    public string PlanName { get; set; }
    public string PeriodType { get; set; } = "MONTH";
    public string PeriodName { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public ulong? WorkshopId { get; set; }
    public string WorkshopName { get; set; }
    public int Status { get; set; } = 2; // 1=Draft, 2=Approved, 3=InProgress, 4=Completed, 5=Cancelled
    public decimal TotalQuantity { get; set; }
    public string Remark { get; set; }

    public virtual ICollection<MasterProductionPlanItem> Items { get; set; } = new List<MasterProductionPlanItem>();
}
