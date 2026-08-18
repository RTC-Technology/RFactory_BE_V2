namespace RFactory.Application.Modules.Warehouses.DTOs;

public class WarehouseLocationDto
{
    public ulong Id { get; set; }
    public long? WarehouseZoneId { get; set; }
    public string WarehouseLocationCode { get; set; } = string.Empty;
    public string WarehouseLocationName { get; set; } = string.Empty;
    public decimal? MaxCapacity { get; set; }
    public bool? IsPickingLocation { get; set; }
    public bool? IsActive { get; set; }
    public ulong? WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseZoneCode { get; set; }
    public string? WarehouseZoneName { get; set; }
}

public class CreateWarehouseLocationRequest
{
    public long? WarehouseZoneId { get; set; }
    public string WarehouseLocationCode { get; set; } = string.Empty;
    public string WarehouseLocationName { get; set; } = string.Empty;
    public decimal? MaxCapacity { get; set; }
    public bool? IsPickingLocation { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Every field is required on update: AutoMapper maps the whole request onto the entity, so
/// an omitted property is written as null rather than left alone. There is no PATCH here.
/// </summary>
public class UpdateWarehouseLocationRequest
{
    public long? WarehouseZoneId { get; set; }
    public string WarehouseLocationCode { get; set; } = string.Empty;
    public string WarehouseLocationName { get; set; } = string.Empty;
    public decimal? MaxCapacity { get; set; }
    public bool? IsPickingLocation { get; set; }
    public bool? IsActive { get; set; }
}
