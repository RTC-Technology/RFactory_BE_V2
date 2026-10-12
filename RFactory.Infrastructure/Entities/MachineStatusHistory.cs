using System;

namespace RFactory.Infrastructure.Entities;

public partial class MachineStatusHistory : TableBase
{
    public ulong MachineId { get; set; }
    public string Status { get; set; } = "IDLE"; // RUNNING, IDLE, DOWNTIME, CHANGEOVER, MAINTENANCE, OFFLINE
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public long? DurationSeconds { get; set; }

    public ulong? ReasonId { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonName { get; set; }
    public string? OperatorName { get; set; }
    public string? WorkOrderCode { get; set; }
    public string? Remarks { get; set; }
}
