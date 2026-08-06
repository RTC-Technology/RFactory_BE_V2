namespace RFactory.Application.Modules.Product.DTOs;

// ─── Product type ────────────────────────────────────────────────────────────

public class ProductTypeDto
{
    public ulong Id { get; set; }
    public string ProductTypeCode { get; set; } = string.Empty;
    public string ProductTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductTypeRequest
{
    public string ProductTypeCode { get; set; } = string.Empty;
    public string ProductTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateProductTypeRequest
{
    public string ProductTypeCode { get; set; } = string.Empty;
    public string ProductTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
}

// ─── Product ─────────────────────────────────────────────────────────────────

public class ProductDto
{
    public ulong Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public long? ProductTypeId { get; set; }
    public long? DefaultUnitId { get; set; }
    public string? DrawingNo { get; set; }
    public string? DrawingPath { get; set; }
    public int? Status { get; set; }
}

public class CreateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public long? ProductTypeId { get; set; }
    public long? DefaultUnitId { get; set; }
    public string? DrawingNo { get; set; }
    public string? DrawingPath { get; set; }
    public int? Status { get; set; }
}

public class UpdateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public long? ProductTypeId { get; set; }
    public long? DefaultUnitId { get; set; }
    public string? DrawingNo { get; set; }
    public string? DrawingPath { get; set; }
    public int? Status { get; set; }
}

// ─── BOM ─────────────────────────────────────────────────────────────────────

/// <summary>
/// A bill of materials belongs to one product and carries a version, so a product can
/// hold several revisions at once with one of them flagged active.
/// </summary>
public class BomDto
{
    public ulong Id { get; set; }
    public long? ProductId { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public string BomName { get; set; } = string.Empty;
    public string? Version { get; set; }
    public int? Status { get; set; }
    public bool IsActive { get; set; }
}

public class CreateBomRequest
{
    public long? ProductId { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public string BomName { get; set; } = string.Empty;
    public string? Version { get; set; }
    public int? Status { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateBomRequest
{
    public long? ProductId { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public string BomName { get; set; } = string.Empty;
    public string? Version { get; set; }
    public int? Status { get; set; }
    public bool IsActive { get; set; }
}

// ─── BOM line ────────────────────────────────────────────────────────────────

/// <summary>
/// One component inside a BOM. <see cref="ProductId"/> is the component, not the product
/// the BOM belongs to — that one lives on <see cref="BomDto.ProductId"/>.
/// </summary>
public class BomDetailDto
{
    public ulong Id { get; set; }
    public long? BomId { get; set; }
    public long? ProductId { get; set; }
    public decimal? Quantity { get; set; }
    public long? UnitId { get; set; }
    /// <summary>Proportional loss, as a percentage of <see cref="Quantity"/>.</summary>
    public decimal? ScrapRate { get; set; }
    /// <summary>Loss that does not scale with quantity, e.g. set-up pieces.</summary>
    public int? FixedScrapQty { get; set; }
}

public class CreateBomDetailRequest
{
    public long? BomId { get; set; }
    public long? ProductId { get; set; }
    public decimal? Quantity { get; set; }
    public long? UnitId { get; set; }
    public decimal? ScrapRate { get; set; }
    public int? FixedScrapQty { get; set; }
}

public class UpdateBomDetailRequest
{
    public long? BomId { get; set; }
    public long? ProductId { get; set; }
    public decimal? Quantity { get; set; }
    public long? UnitId { get; set; }
    public decimal? ScrapRate { get; set; }
    public int? FixedScrapQty { get; set; }
}
