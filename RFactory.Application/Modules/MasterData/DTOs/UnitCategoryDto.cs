namespace RFactory.Application.Modules.MasterData.DTOs;

/// <summary>A family of comparable units — mass, length, time. Only units inside the
/// same category can sensibly convert into one another.</summary>
public class UnitCategoryDto
{
    public ulong Id { get; set; }
    public string UnitCategoryCode { get; set; } = string.Empty;
    public string UnitCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUnitCategoryRequest
{
    public string UnitCategoryCode { get; set; } = string.Empty;
    public string UnitCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateUnitCategoryRequest
{
    public string UnitCategoryCode { get; set; } = string.Empty;
    public string UnitCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// A conversion between two units.
///
/// <see cref="FormulaType"/> is a plain int in the schema with no enum behind it, and the
/// table carries both a multiplier and a divisor — the pair is read as a ratio, so one
/// <c>FromUnit</c> equals <c>MultiplyValue / DivideValue</c> of <c>ToUnit</c>.
/// </summary>
public class UnitConversionDto
{
    public ulong Id { get; set; }
    public long? FromUnitId { get; set; }
    public long? ToUnitId { get; set; }
    public decimal? MultiplyValue { get; set; }
    public decimal? DivideValue { get; set; }
    public int FormulaType { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUnitConversionRequest
{
    public long? FromUnitId { get; set; }
    public long? ToUnitId { get; set; }
    public decimal? MultiplyValue { get; set; }
    public decimal? DivideValue { get; set; }
    public int FormulaType { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateUnitConversionRequest
{
    public long? FromUnitId { get; set; }
    public long? ToUnitId { get; set; }
    public decimal? MultiplyValue { get; set; }
    public decimal? DivideValue { get; set; }
    public int FormulaType { get; set; }
    public bool IsActive { get; set; }
}
