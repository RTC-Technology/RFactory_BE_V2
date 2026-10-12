namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Packaging specification defining unit-to-carton-to-pallet packaging rules, dimensions, weights, and labels.
/// </summary>
public partial class PackingSpecification : TableBase
{
    public string SpecCode { get; set; } = string.Empty;
    public string SpecName { get; set; } = string.Empty;
    public ulong? ProductId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string PackagingType { get; set; } = "CARTON"; // BOX, CARTON, PALLET, CRATE, BUNDLE
    public decimal QuantityPerPack { get; set; } = 1;
    public ulong? PackUnitId { get; set; }

    // Physical dimensions & weights
    public decimal LengthMm { get; set; } = 0;
    public decimal WidthMm { get; set; } = 0;
    public decimal HeightMm { get; set; } = 0;
    public decimal VolumeM3 { get; set; } = 0;
    public decimal NetWeightKg { get; set; } = 0;
    public decimal GrossWeightKg { get; set; } = 0;
    public decimal TareWeightKg { get; set; } = 0;

    // Multi-tier hierarchy (Inner -> Master -> Pallet)
    public decimal? InnerPackQty { get; set; }
    public decimal? MasterCartonQty { get; set; }
    public int? PalletTi { get; set; } // Cartons per layer
    public int? PalletHi { get; set; } // Layers per pallet
    public decimal? TotalUnitsPerPallet { get; set; }

    // Links & specs
    public ulong? LabelTemplateId { get; set; }
    public string? Barcode { get; set; }
    public string? PackagingMaterial { get; set; }
    public string? Instructions { get; set; }
    public ulong IsDefault { get; set; } = 0;
    public string Status { get; set; } = "ACTIVE"; // ACTIVE, DRAFT, OBSOLETE
}
