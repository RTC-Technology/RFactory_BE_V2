using System;

namespace RFactory.Infrastructure.Entities;

public partial class MachineDefectLog : TableBase
{
    public ulong MachineId { get; set; }
    public string DefectType { get; set; } = "SCRAP"; // SCRAP, REWORK
    public ulong? ReasonId { get; set; }
    public string? ReasonCode { get; set; }
    public string? ReasonName { get; set; }
    public decimal Quantity { get; set; } = 1m;
    public ulong? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? WorkOrderCode { get; set; }
    public DateTime LoggedTime { get; set; } = DateTime.UtcNow;
    public string? OperatorName { get; set; }
    public string? Remarks { get; set; }
}
