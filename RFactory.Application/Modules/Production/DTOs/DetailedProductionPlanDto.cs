using System;
using System.Collections.Generic;

namespace RFactory.Application.Modules.Production.DTOs;

#region Master Plan DTOs

public class MasterProductionPlanDto
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PeriodType { get; set; } = "MONTH";
    public string? PeriodName { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public ulong? WorkshopId { get; set; }
    public string? WorkshopName { get; set; }
    public int Status { get; set; }
    public string StatusName => Status switch
    {
        1 => "Dự thảo",
        2 => "Đã duyệt",
        3 => "Đang thực hiện",
        4 => "Hoàn thành",
        5 => "Đã hủy",
        _ => "Không xác định"
    };
    public decimal TotalQuantity { get; set; }
    public string? Remark { get; set; }
    public List<MasterProductionPlanItemDto> Items { get; set; } = new();
}

public class MasterProductionPlanItemDto
{
    public ulong Id { get; set; }
    public ulong MasterPlanId { get; set; }
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public ulong? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public decimal DailyCapacity { get; set; } = 100;
    public string? Remark { get; set; }
}

#endregion

#region Detailed Plan DTOs

public class DetailedProductionPlanDto
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public ulong? MasterPlanId { get; set; }
    public string? MasterPlanCode { get; set; }
    public ulong? WorkshopId { get; set; }
    public string? WorkshopName { get; set; }
    public ulong? LineId { get; set; }
    public string? LineName { get; set; }
    public ulong? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string PeriodType { get; set; } = "DAY"; // DAY, SHIFT
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string AllocationMethod { get; set; } = "FORWARD"; // FORWARD, BACKWARD, MANUAL
    public int Status { get; set; } = 1; // 1=Draft, 2=Approved, 3=InProgress, 4=Completed, 5=Cancelled
    public string StatusName => Status switch
    {
        1 => "Dự thảo",
        2 => "Đã duyệt",
        3 => "Đang thực hiện",
        4 => "Hoàn thành",
        5 => "Đã hủy",
        _ => "Không xác định"
    };
    public decimal TotalRequiredQuantity { get; set; }
    public decimal TotalPlannedQuantity { get; set; }
    public int ProductionOrderCount { get; set; }
    public string? Remark { get; set; }

    public List<DetailedProductionPlanItemDto> Items { get; set; } = new();
    public List<DetailedProductionPlanScheduleDto> Schedules { get; set; } = new();
}

public class DetailedProductionPlanItemDto
{
    public ulong Id { get; set; }
    public ulong PlanId { get; set; }
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public ulong? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DailyCapacity { get; set; } = 100;
    public string? SalesOrderNo { get; set; }
    public int Sequence { get; set; } = 1;
    public string? Remark { get; set; }
    public decimal RemainingToPlan => RequiredQuantity - PlannedQuantity;
}

public class DetailedProductionPlanScheduleDto
{
    public ulong Id { get; set; }
    public ulong PlanId { get; set; }
    public ulong PlanItemId { get; set; }
    public ulong ProductId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public ulong? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public decimal Capacity { get; set; }
    public decimal PlannedQuantity { get; set; }
    public bool IsDayOff { get; set; }
    public bool IsOvertime { get; set; }
    public ulong? ProductionOrderId { get; set; }
    public string? ProductionOrderNo { get; set; }
    public string? Remark { get; set; }
}

#endregion

#region Request / Filter / Response DTOs

public class DetailedProductionPlanFilterDto
{
    public string? Keyword { get; set; }
    public ulong? WorkshopId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class DetailedProductionPlanCreateRequest
{
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public ulong? MasterPlanId { get; set; }
    public string? MasterPlanCode { get; set; }
    public ulong? WorkshopId { get; set; }
    public string? WorkshopName { get; set; }
    public ulong? LineId { get; set; }
    public string? LineName { get; set; }
    public ulong? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string PeriodType { get; set; } = "DAY"; // DAY, SHIFT
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string AllocationMethod { get; set; } = "FORWARD";
    public string? Remark { get; set; }

    public List<DetailedProductionPlanItemRequest> Items { get; set; } = new();
    public List<DetailedProductionPlanScheduleRequest> Schedules { get; set; } = new();
}

public class DetailedProductionPlanUpdateRequest
{
    public string PlanName { get; set; } = string.Empty;
    public ulong? MasterPlanId { get; set; }
    public string? MasterPlanCode { get; set; }
    public ulong? WorkshopId { get; set; }
    public string? WorkshopName { get; set; }
    public ulong? LineId { get; set; }
    public string? LineName { get; set; }
    public ulong? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string PeriodType { get; set; } = "DAY";
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string AllocationMethod { get; set; } = "FORWARD";
    public string? Remark { get; set; }

    public List<DetailedProductionPlanItemRequest> Items { get; set; } = new();
    public List<DetailedProductionPlanScheduleRequest> Schedules { get; set; } = new();
}

public class DetailedProductionPlanItemRequest
{
    public ulong? Id { get; set; }
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public ulong? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal DailyCapacity { get; set; } = 100;
    public string? SalesOrderNo { get; set; }
    public int Sequence { get; set; } = 1;
    public string? Remark { get; set; }
}

public class DetailedProductionPlanScheduleRequest
{
    public ulong? Id { get; set; }
    public ulong? PlanItemId { get; set; }
    public ulong ProductId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public ulong? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public decimal Capacity { get; set; }
    public decimal PlannedQuantity { get; set; }
    public bool IsDayOff { get; set; }
    public bool IsOvertime { get; set; }
    public string? Remark { get; set; }
}

#endregion

#region Auto-Allocation & Calculation DTOs

public class PlanAllocationCalculationRequest
{
    public string Method { get; set; } = "FORWARD"; // FORWARD or BACKWARD
    public string PeriodType { get; set; } = "DAY"; // DAY or SHIFT
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<PlanAllocationItemInput> Items { get; set; } = new();
    public List<string> Shifts { get; set; } = new(); // e.g. ["Ca 1", "Ca 2", "Ca 3"]
    public List<DayOffOverride> DayOffOverrides { get; set; } = new();
}

public class PlanAllocationItemInput
{
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal DailyCapacity { get; set; } = 100;
}

public class DayOffOverride
{
    public DateTime Date { get; set; }
    public bool IsDayOff { get; set; }
    public bool IsOvertime { get; set; }
}

public class PlanAllocationCalculationResultDto
{
    public List<DetailedProductionPlanScheduleDto> Schedules { get; set; } = new();
    public List<PlanItemAllocatedSummaryDto> ItemSummaries { get; set; } = new();
    public bool IsFullyAllocated { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class PlanItemAllocatedSummaryDto
{
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public bool IsMatched => RequiredQuantity == AllocatedQuantity;
}

public class ValidateQuantitiesRequest
{
    public List<PlanAllocationItemInput> Items { get; set; } = new();
    public List<DetailedProductionPlanScheduleDto> Schedules { get; set; } = new();
}

public class ValidateQuantitiesResultDto
{
    public bool IsValid { get; set; }
    public string SummaryMessage { get; set; } = string.Empty;
    public List<QuantityComparisonItemDto> Details { get; set; } = new();
}

public class QuantityComparisonItemDto
{
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal Difference => PlannedQuantity - RequiredQuantity;
    public string Status => Difference switch
    {
        0 => "MATCH",      // Khớp 100%
        < 0 => "DEFICIT",  // Thiếu
        _ => "EXCESS"      // Thừa
    };
    public string StatusLabel => Difference switch
    {
        0 => "Khớp 100%",
        < 0 => $"Thiếu {Math.Abs(Difference)}",
        _ => $"Thừa {Difference}"
    };
}

public class GenerateOrdersFromPlanRequest
{
    public ulong PlanId { get; set; }
    public List<ulong>? ProductIds { get; set; }
    public string GroupBy { get; set; } = "DAY"; // "DAY" (tạo lệnh theo ngày có số lượng) hoặc "ITEM" (tổng theo sản phẩm cả kỳ)
    public bool AutoApprove { get; set; } = false;
    public string? Remark { get; set; }
}

public class GenerateOrdersResultDto
{
    public int GeneratedCount { get; set; }
    public List<GeneratedOrderItemDto> GeneratedOrders { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class GeneratedOrderItemDto
{
    public ulong OrderId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public ulong ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public string Status { get; set; } = "Draft";
}

#endregion
