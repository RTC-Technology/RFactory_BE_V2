namespace RFactory.Application.Modules.MasterData.DTOs;

public class LabelTemplateDto
{
    public ulong Id { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public decimal WidthMm { get; set; }
    public decimal HeightMm { get; set; }
    public string Orientation { get; set; } = "LANDSCAPE";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public bool ShowQr { get; set; }
    public bool ShowBarcode { get; set; }
    public bool ShowLogo { get; set; }
    public bool ShowBorder { get; set; }
    public int QrSizePx { get; set; }
    public string? FieldsConfigJson { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class LabelTemplateRequest
{
    public string TemplateCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public decimal WidthMm { get; set; } = 70.00m;
    public decimal HeightMm { get; set; } = 50.00m;
    public string Orientation { get; set; } = "LANDSCAPE";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public bool ShowQr { get; set; } = true;
    public bool ShowBarcode { get; set; } = false;
    public bool ShowLogo { get; set; } = true;
    public bool ShowBorder { get; set; } = true;
    public int QrSizePx { get; set; } = 150;
    public string? FieldsConfigJson { get; set; }
}
