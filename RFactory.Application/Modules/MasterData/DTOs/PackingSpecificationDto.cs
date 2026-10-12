namespace RFactory.Application.Modules.MasterData.DTOs;

public class PackingSpecificationDto
{
    public ulong Id { get; set; }
    public string SpecCode { get; set; } = string.Empty;
    public string SpecName { get; set; } = string.Empty;
    public ulong? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string PackagingType { get; set; } = "CARTON";
    public decimal QuantityPerPack { get; set; }
    public ulong? PackUnitId { get; set; }
    public string? PackUnitName { get; set; }

    // Dimensions & weights
    public decimal LengthMm { get; set; }
    public decimal WidthMm { get; set; }
    public decimal HeightMm { get; set; }
    public decimal VolumeM3 { get; set; }
    public decimal NetWeightKg { get; set; }
    public decimal GrossWeightKg { get; set; }
    public decimal TareWeightKg { get; set; }

    // Multi-tier hierarchy
    public decimal? InnerPackQty { get; set; }
    public decimal? MasterCartonQty { get; set; }
    public int? PalletTi { get; set; }
    public int? PalletHi { get; set; }
    public decimal? TotalUnitsPerPallet { get; set; }

    // Links & specs
    public ulong? LabelTemplateId { get; set; }
    public string? LabelTemplateName { get; set; }
    public string? Barcode { get; set; }
    public string? PackagingMaterial { get; set; }
    public string? Instructions { get; set; }
    public bool IsDefault { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedDate { get; set; }
}

public class PackingSpecificationRequest
{
    public string SpecCode { get; set; } = string.Empty;
    public string SpecName { get; set; } = string.Empty;
    public ulong? ProductId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string PackagingType { get; set; } = "CARTON";
    public decimal QuantityPerPack { get; set; } = 1;
    public ulong? PackUnitId { get; set; }

    public decimal LengthMm { get; set; }
    public decimal WidthMm { get; set; }
    public decimal HeightMm { get; set; }
    public decimal VolumeM3 { get; set; }
    public decimal NetWeightKg { get; set; }
    public decimal GrossWeightKg { get; set; }
    public decimal TareWeightKg { get; set; }

    public decimal? InnerPackQty { get; set; }
    public decimal? MasterCartonQty { get; set; }
    public int? PalletTi { get; set; }
    public int? PalletHi { get; set; }
    public decimal? TotalUnitsPerPallet { get; set; }

    public ulong? LabelTemplateId { get; set; }
    public string? Barcode { get; set; }
    public string? PackagingMaterial { get; set; }
    public string? Instructions { get; set; }
    public bool IsDefault { get; set; }
    public string Status { get; set; } = "ACTIVE";
}
