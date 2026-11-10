#nullable disable
using System;
using System.Collections.Generic;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Kế hoạch sản xuất chi tiết (Detailed Production Plan)
/// </summary>
public partial class DetailedProductionPlan
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public string PlanCode { get; set; }
    public string PlanName { get; set; }
    public ulong? MasterPlanId { get; set; }
    public string MasterPlanCode { get; set; }
    public ulong? WorkshopId { get; set; }
    public string WorkshopName { get; set; }
    public ulong? LineId { get; set; }
    public string LineName { get; set; }
    public ulong? DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string PeriodType { get; set; } = "DAY"; // DAY, SHIFT
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string AllocationMethod { get; set; } = "FORWARD"; // FORWARD, BACKWARD, MANUAL
    public int Status { get; set; } = 1; // 1=Draft, 2=Approved, 3=InProgress, 4=Completed, 5=Cancelled
    public decimal TotalRequiredQuantity { get; set; }
    public decimal TotalPlannedQuantity { get; set; }
    public int ProductionOrderCount { get; set; }
    public string Remark { get; set; }

    public virtual ICollection<DetailedProductionPlanItem> Items { get; set; } = new List<DetailedProductionPlanItem>();
    public virtual ICollection<DetailedProductionPlanSchedule> Schedules { get; set; } = new List<DetailedProductionPlanSchedule>();
}
