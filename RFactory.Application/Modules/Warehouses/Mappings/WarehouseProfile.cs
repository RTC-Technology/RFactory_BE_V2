using AutoMapper;
using RFactory.Application.Modules.Warehouses.DTOs;

namespace RFactory.Application.Modules.Warehouses.Mappings;

public class WarehouseProfile : Profile
{
    public WarehouseProfile()
    {
        CreateMap<Infrastructure.Entities.Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseRequest, Infrastructure.Entities.Warehouse>();
        CreateMap<UpdateWarehouseRequest, Infrastructure.Entities.Warehouse>();

        CreateMap<Infrastructure.Entities.WarehouseLocation, WarehouseLocationDto>();
        CreateMap<CreateWarehouseLocationRequest, Infrastructure.Entities.WarehouseLocation>();
        CreateMap<UpdateWarehouseLocationRequest, Infrastructure.Entities.WarehouseLocation>();

        CreateMap<Infrastructure.Entities.WarehouseZone, WarehouseZoneDto>();
        CreateMap<CreateWarehouseZoneRequest, Infrastructure.Entities.WarehouseZone>();
        CreateMap<UpdateWarehouseZoneRequest, Infrastructure.Entities.WarehouseZone>();
    }
}