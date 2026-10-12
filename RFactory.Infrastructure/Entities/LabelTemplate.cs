namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Reusable print layout template for barcode &amp; QR labels across MES master entities.
/// </summary>
public partial class LabelTemplate : TableBase
{
    public string TemplateCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // PRODUCT, MACHINE, LOCATION, EMPLOYEE
    public decimal WidthMm { get; set; } = 70.00m;
    public decimal HeightMm { get; set; } = 50.00m;
    public string Orientation { get; set; } = "LANDSCAPE"; // PORTRAIT, LANDSCAPE
    public ulong IsDefault { get; set; } = 0;
    public ulong IsActive { get; set; } = 1;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public ulong ShowQr { get; set; } = 1;
    public ulong ShowBarcode { get; set; } = 0;
    public ulong ShowLogo { get; set; } = 1;
    public ulong ShowBorder { get; set; } = 1;
    public int QrSizePx { get; set; } = 150;
    public string? FieldsConfigJson { get; set; }
}
