using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Infrastructure.Entities;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Inventory.Mappings;

internal class InventoryProfile:Profile
{
    public InventoryProfile()
    {
        CreateMap<Entities.Inventory, InventoryDto>();
        //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id == true));
        CreateMap<CreateInventoryRequest, Entities.Inventory>();
        CreateMap<UpdateInventoryRequest, Entities.Inventory>();


        CreateMap<Entities.InventoryTransaction, InventoryTransactionDto>();
        CreateMap<CreateInventoryTransactionRequest, Entities.InventoryTransaction>();
        CreateMap<UpdateInventoryTransactionRequest, Entities.InventoryTransaction>();
    }
}


