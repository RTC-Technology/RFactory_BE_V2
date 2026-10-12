namespace RFactory.Infrastructure.Entities;

public partial class MachineReasonCode : TableBase
{
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonName { get; set; } = string.Empty;
    public string ReasonType { get; set; } = "DOWNTIME"; // DOWNTIME, SCRAP, REWORK
    public string Category { get; set; } = "General";
    public ulong IsPlanned { get; set; } = 0UL;
    public ulong IsActive { get; set; } = 1UL;
    public string? Description { get; set; }
}
