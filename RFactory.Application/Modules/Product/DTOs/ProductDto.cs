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

// ─── Product Group─────────────────────────────────────────────────────────────────
public class ProductGroupDto
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string GroupNo { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public ulong? ParentId { get; set; }
}

public class ProductGroupRequest
{
    public string GroupNo { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public ulong? ParentId { get; set; }
}


// ─── Product ─────────────────────────────────────────────────────────────────

public class ProductDto
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string ProductCode { get; set; } = string.Empty ;
    public string ProductName { get; set; } = string.Empty;
    public long? ProductTypeId { get; set; }
    public long? DefaultUnitId { get; set; }
    public string DrawingNo { get; set; } = string.Empty;
    public string DrawingPath { get; set; } = string.Empty;
    public int? Status { get; set; }
    public int? ProductNature { get; set; }
    public long? ProductGroupId { get; set; }
    public long? ProductionUnitId { get; set; }
    public long? DefaultWarehouseId { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public decimal? FixedPurchasePrice { get; set; }
    public decimal? WastageRate { get; set; }
    public decimal? PreparationTime { get; set; }
    public decimal? WarrantyPeriod { get; set; }
    public int? WarrantyPeriodUnit { get; set; }
    public decimal? VatRate { get; set; }
    public decimal? StandardProductionTime { get; set; }
    public bool IsOutsourced { get; set; }
    public string? Description { get; set; }
    public string? ProductionColor { get; set; }
}

public class ProductRequest
{
    public string? ProductCode { get; set; } 
    public string? ProductName { get; set; }
    public long? ProductTypeId { get; set; }
    public long? DefaultUnitId { get; set; }
    public string? DrawingNo { get; set; } 
    public string? DrawingPath { get; set; } 
    public int? Status { get; set; }
    public int? ProductNature { get; set; }
    public long? ProductGroupId { get; set; }
    public long? ProductionUnitId { get; set; }
    public long? DefaultWarehouseId { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public decimal? FixedPurchasePrice { get; set; }
    public decimal? WastageRate { get; set; }
    public decimal? PreparationTime { get; set; }
    public decimal? WarrantyPeriod { get; set; }
    public int? WarrantyPeriodUnit { get; set; }
    public decimal? VatRate { get; set; }
    public decimal? StandardProductionTime { get; set; }
    public bool IsOutsourced { get; set; }
    public string? Description { get; set; }
    public string? ProductionColor { get; set; }
    public List<BomRequest>? Boms { get; set; }
    public List<RoutingRequest>? Routings { get; set; }
}

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

public class BomRequest
{
    public ulong Id { get; set; }
    public long? ProductId { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public string BomName { get; set; } = string.Empty;
    public string? Version { get; set; }
    public int? Status { get; set; }
    public bool IsActive { get; set; } = true;
    public List<BomDetailRequest>? BomDetails { get; set; }
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

public class BomDetailRequest
{
    public ulong Id { get; set; }
    public long? BomId { get; set; }
    public long? ProductId { get; set; }
    public decimal? Quantity { get; set; }
    public long? UnitId { get; set; }
    public decimal? ScrapRate { get; set; }
    public int? FixedScrapQty { get; set; }
}

// ─── Routing (process specification) ────────────────────────────────────────

/// <summary>
/// A process specification (routing) belongs to one product and carries a version, so a
/// product can hold several revisions with one of them flagged active. Mirrors Bom.
/// </summary>
public class RoutingDto
{
    public ulong Id { get; set; }
    public long? ProductId { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class RoutingRequest
{
    public ulong Id { get; set; }
    public long? ProductId { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<RoutingOperationRequest>? RoutingOperations { get; set; }
}

// ─── Routing operation (công đoạn) ──────────────────────────────────────────

/// <summary>
/// One work step inside a routing. <see cref="RoutingId"/> is the routing it belongs to;
/// <see cref="Sequence"/> orders the steps. Mirrors BomDetail.
/// </summary>
public class RoutingOperationDto
{
    public ulong Id { get; set; }
    public long? RoutingId { get; set; }
    public int? Sequence { get; set; }
    public string RoutingOperationCode { get; set; } = string.Empty;
    public string RoutingOperationName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsFinishOperation { get; set; }
    public bool IsOutputOperation { get; set; }
}

public class RoutingOperationRequest
{
    public ulong Id { get; set; }
    public long? RoutingId { get; set; }
    public int? Sequence { get; set; }
    public string RoutingOperationCode { get; set; } = string.Empty;
    public string RoutingOperationName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsFinishOperation { get; set; }
    public bool IsOutputOperation { get; set; }
}

