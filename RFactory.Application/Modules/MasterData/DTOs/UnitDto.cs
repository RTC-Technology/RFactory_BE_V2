namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>
/// Unit of measure. Read by the product and BOM screens to label quantities.
/// <c>IsBaseUnit</c> is a nullable bit(1) (ulong?) exposed as a plain bool.
/// </summary>
public class UnitDto
{
    public ulong Id { get; set; }
    public long? UnitCategoryId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public int? DecimalPlaces { get; set; }
    public bool IsBaseUnit { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUnitRequest
{
    public long? UnitCategoryId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public int? DecimalPlaces { get; set; }
    public bool IsBaseUnit { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateUnitRequest
{
    public long? UnitCategoryId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public int? DecimalPlaces { get; set; }
    public bool IsBaseUnit { get; set; }
    public bool IsActive { get; set; }
}
