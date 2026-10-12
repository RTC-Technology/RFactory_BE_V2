namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Configurable numbering rule for packages, cartons, pallets, and packing specifications.
/// Formats prefix, date masks, sequence padding, and reset periods.
/// </summary>
public partial class PackingNumberingRule : TableBase
{
    public string RuleCode { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string TargetScope { get; set; } = "CARTON"; // CARTON, PALLET, INNER_BOX, SPEC, LOT

    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public string DateFormat { get; set; } = "YYYYMMDD"; // NONE, YYYYMMDD, YYMMDD, YYYYMM, YYWW
    public int SequenceLength { get; set; } = 4;
    public long CurrentSequence { get; set; } = 0;
    public string ResetPeriod { get; set; } = "DAILY"; // NEVER, DAILY, MONTHLY, YEARLY
    public DateTime? LastResetDate { get; set; }

    public ulong IncludeCheckDigit { get; set; } = 0; // 1 = Modulo 10 GS1 check digit
    public ulong IsDefault { get; set; } = 0;
    public ulong IsActive { get; set; } = 1;
    public string? Description { get; set; }
}
