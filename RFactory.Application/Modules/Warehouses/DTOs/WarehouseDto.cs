using RFactory.Application.Modules.Warehouses.DTOs;

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

public class UpdateWarehouseRequest
{
    public long? FactoryId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int? WarehouseType { get; set; }
    public bool? IsActive { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class WarehouseLocationDto
{
    public ulong Id { get; set; }
    public long? WarehouseZoneId { get; set; }
    public string WarehouseLocationCode { get; set; } = string.Empty;
    public string WarehouseLocationName { get; set; } = string.Empty;
    public decimal? MaxCapacity { get; set; }
    public bool? IsPickingLocation { get; set; }
    public bool? IsActive { get; set; }
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

public class UpdateWarehouseLocationRequest
{
    public long? WarehouseZoneId { get; set; }
    public string WarehouseLocationCode { get; set; } = string.Empty;
    public string WarehouseLocationName { get; set; } = string.Empty;
    public decimal? MaxCapacity { get; set; }
    public bool? IsPickingLocation { get; set; }
    public bool? IsActive { get; set; }
}

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

public class UpdateWarehouseZoneRequest
{
    public long? WarehouseId { get; set; }
    public string WarehouseZoneCode { get; set; } = string.Empty;
    public string WarehouseZoneName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}