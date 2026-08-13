namespace RFactory.Application.Modules.Warehouses.DTOs;

public class WarehouseDto
{
    public ulong Id { get; set; }
    public long? FactoryId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int? WarehouseType { get; set; }
    public bool? IsActive { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateWarehouseRequest
{
    public long? FactoryId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int? WarehouseType { get; set; }
    public bool? IsActive { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Every field is required on update: AutoMapper maps the whole request onto the entity, so
/// an omitted property is written as null rather than left alone. There is no PATCH here.
/// </summary>
public class UpdateWarehouseRequest
{
    public long? FactoryId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int? WarehouseType { get; set; }
    public bool? IsActive { get; set; }
    public string Description { get; set; } = string.Empty;
}
