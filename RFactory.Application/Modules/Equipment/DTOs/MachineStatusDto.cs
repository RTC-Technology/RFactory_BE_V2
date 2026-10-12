using System;
using System.Collections.Generic;

namespace RFactory.Application.Modules.Equipment.DTOs;

public class MachineReasonCodeDto
{
    public ulong Id { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonName { get; set; } = string.Empty;
    public string ReasonType { get; set; } = "DOWNTIME"; // DOWNTIME, SCRAP, REWORK
    public string Category { get; set; } = "General";
    public bool IsPlanned { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class MachineReasonCodeRequest
{
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonName { get; set; } = string.Empty;
    public string ReasonType { get; set; } = "DOWNTIME";
    public string Category { get; set; } = "General";
    public bool IsPlanned { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}

public class MachineRealtimeStatusDto
{
    public ulong MachineId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public long? LineId { get; set; }
    public string LineName { get; set; } = string.Empty;
    public long? MachineTypeId { get; set; }
    public string MachineTypeName { get; set; } = string.Empty;

    public string Status { get; set; } = "IDLE"; // RUNNING, IDLE, DOWNTIME, CHANGEOVER, MAINTENANCE, OFFLINE
    public DateTime StatusStartTime { get; set; }
    public long DurationSeconds { get; set; }
    public double DurationMinutes { get; set; }

    public ulong? CurrentReasonId { get; set; }
    public string? CurrentReasonCode { get; set; }
    public string? CurrentReasonName { get; set; }
    public string? ReasonCategory { get; set; }
    public bool? IsPlannedDowntime { get; set; }

    public string? OperatorName { get; set; }
    public string? WorkOrderCode { get; set; }
    public string? Remarks { get; set; }

    public double TodayDowntimeMinutes { get; set; }
    public decimal TodayScrapQty { get; set; }
    public decimal TodayReworkQty { get; set; }
}

public class ChangeMachineStatusRequest
{
    public ulong MachineId { get; set; }
    public string NewStatus { get; set; } = "RUNNING"; // RUNNING, IDLE, DOWNTIME, CHANGEOVER, MAINTENANCE, OFFLINE
    public ulong? ReasonId { get; set; }
    public string? OperatorName { get; set; }
    public string? WorkOrderCode { get; set; }
    public string? Remarks { get; set; }
}

public class MachineStatusHistoryDto
{
    public ulong Id { get; set; }
    public ulong MachineId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public long? DurationSeconds { get; set; }
    public double DurationMinutes { get; set; }

    public ulong? ReasonId { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonName { get; set; }
    public string? OperatorName { get; set; }
    public string? WorkOrderCode { get; set; }
    public string? Remarks { get; set; }
}

public class LogMachineDefectRequest
{
    public ulong MachineId { get; set; }
    public string DefectType { get; set; } = "SCRAP"; // SCRAP, REWORK
    public ulong? ReasonId { get; set; }
    public decimal Quantity { get; set; } = 1m;
    public ulong? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? WorkOrderCode { get; set; }
    public string? OperatorName { get; set; }
    public string? Remarks { get; set; }
}

public class MachineDefectLogDto
{
    public ulong Id { get; set; }
    public ulong MachineId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string DefectType { get; set; } = "SCRAP";
    public ulong? ReasonId { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonName { get; set; }
    public decimal Quantity { get; set; }
    public ulong? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? WorkOrderCode { get; set; }
    public DateTime LoggedTime { get; set; }
    public string? OperatorName { get; set; }
    public string? Remarks { get; set; }
}

public class MachineStatusDashboardSummaryDto
{
    public int TotalMachines { get; set; }
    public int RunningCount { get; set; }
    public int IdleCount { get; set; }
    public int DowntimeCount { get; set; }
    public int ChangeoverCount { get; set; }
    public int MaintenanceCount { get; set; }
    public int OfflineCount { get; set; }
    public double AvailabilityPercentage { get; set; }
    public double TotalDowntimeMinutesToday { get; set; }
    public decimal TotalScrapQtyToday { get; set; }
    public decimal TotalReworkQtyToday { get; set; }
}
