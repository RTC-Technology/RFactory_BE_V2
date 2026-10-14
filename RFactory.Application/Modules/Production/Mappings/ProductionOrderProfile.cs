using AutoMapper;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Mappings;

public class ProductionOrderProfile : Profile
{
    public ProductionOrderProfile()
    {
        CreateMap<ProductionOrder, ProductionOrderDto>()
            .ForMember(d => d.Materials, opt => opt.MapFrom(s => s.Materials))
            .ForMember(d => d.Operations, opt => opt.MapFrom(s => s.Operations));

        CreateMap<ProductionOrderCreateRequest, ProductionOrder>()
            .ForMember(d => d.Materials, opt => opt.Ignore())
            .ForMember(d => d.Operations, opt => opt.Ignore());

        CreateMap<ProductionOrderUpdateRequest, ProductionOrder>()
            .ForMember(d => d.Materials, opt => opt.Ignore())
            .ForMember(d => d.Operations, opt => opt.Ignore());

        CreateMap<ProductionOrderMaterial, ProductionOrderMaterialDto>();
        CreateMap<ProductionOrderMaterialInputDto, ProductionOrderMaterial>();

        CreateMap<ProductionOrderOperation, ProductionOrderOperationDto>();
        CreateMap<ProductionOrderOperationInputDto, ProductionOrderOperation>();
    }
}
