namespace RFactory.Application.Modules.MasterData.DTOs;

public class PackingNumberingRuleDto
{
    public ulong Id { get; set; }
    public string RuleCode { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string TargetScope { get; set; } = "CARTON";
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public string DateFormat { get; set; } = "YYYYMMDD";
    public int SequenceLength { get; set; } = 4;
    public long CurrentSequence { get; set; }
    public string ResetPeriod { get; set; } = "DAILY";
    public DateTime? LastResetDate { get; set; }
    public bool IncludeCheckDigit { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string SamplePreview { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class PackingNumberingRuleRequest
{
    public string RuleCode { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string TargetScope { get; set; } = "CARTON";
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public string DateFormat { get; set; } = "YYYYMMDD";
    public int SequenceLength { get; set; } = 4;
    public long CurrentSequence { get; set; } = 0;
    public string ResetPeriod { get; set; } = "DAILY";
    public bool IncludeCheckDigit { get; set; } = false;
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}

public class GeneratePackingCodeRequest
{
    public string? TargetScope { get; set; } // CARTON, PALLET, INNER_BOX, SPEC, LOT
    public ulong? RuleId { get; set; }
    public int Count { get; set; } = 1; // Number of codes to generate (1 to 100)
}

public class GeneratedPackingCodesDto
{
    public string RuleCode { get; set; } = string.Empty;
    public string TargetScope { get; set; } = string.Empty;
    public List<string> GeneratedCodes { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
