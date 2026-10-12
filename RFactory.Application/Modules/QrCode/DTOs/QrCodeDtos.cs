namespace RFactory.Application.Modules.QrCode.DTOs;

public class QrCodeGenerateRequest
{
    public string EntityType { get; set; } = string.Empty; // PRODUCT, LOCATION, MACHINE, EMPLOYEE, GOODS_RECEIPT, INVENTORY, CUSTOM
    public ulong? EntityId { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
}

public class QrCodeDto
{
    public string Payload { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public ulong? EntityId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string LabelTitle { get; set; } = string.Empty;
    public string LabelSubtitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class QrScanRequest
{
    public string Payload { get; set; } = string.Empty;
}

public class QrScanResultDto
{
    public bool Found { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public ulong? EntityId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new();
    public string ActionRoute { get; set; } = string.Empty;
}

public class QrItemLookupDto
{
    public ulong Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}

public class QrBatchPrintRequest
{
    public string EntityType { get; set; } = string.Empty;
    public List<ulong> EntityIds { get; set; } = new();
}
