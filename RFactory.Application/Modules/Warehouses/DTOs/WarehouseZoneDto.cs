namespace RFactory.Application.Modules.Warehouses.DTOs;

public class WarehouseZoneDto
{
    public ulong Id { get; set; }
    public long? WarehouseId { get; set; }
    public string WarehouseZoneCode { get; set; } = string.Empty;
    public string WarehouseZoneName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateWarehouseZoneRequest
{
    public long? WarehouseId { get; set; }
    public string WarehouseZoneCode { get; set; } = string.Empty;
    public string WarehouseZoneName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Every field is required on update: AutoMapper maps the whole request onto the entity, so
/// an omitted property is written as null rather than left alone. There is no PATCH here.
/// </summary>
public class UpdateWarehouseZoneRequest
{
    public long? WarehouseId { get; set; }
    public string WarehouseZoneCode { get; set; } = string.Empty;
    public string WarehouseZoneName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
