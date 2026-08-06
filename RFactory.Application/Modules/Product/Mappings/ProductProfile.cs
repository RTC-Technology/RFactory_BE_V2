using AutoMapper;
using RFactory.Application.Modules.Product.DTOs;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Product.Mappings;

/// <summary>
/// AutoMapper profile for the Product module. <c>IsActive</c> is a nullable bit(1) on both
/// ProductType and Bom but travels as a plain bool, the same treatment User.IsAdmin gets.
/// </summary>
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Entities.ProductType, ProductTypeDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true));
        CreateMap<CreateProductTypeRequest, Entities.ProductType>();
        CreateMap<UpdateProductTypeRequest, Entities.ProductType>();

        CreateMap<Entities.Product, ProductDto>();
        CreateMap<CreateProductRequest, Entities.Product>();
        CreateMap<UpdateProductRequest, Entities.Product>();

        CreateMap<Entities.Bom, BomDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true));
        CreateMap<CreateBomRequest, Entities.Bom>();
        CreateMap<UpdateBomRequest, Entities.Bom>();

        CreateMap<Entities.BomDetail, BomDetailDto>();
        CreateMap<CreateBomDetailRequest, Entities.BomDetail>();
        CreateMap<UpdateBomDetailRequest, Entities.BomDetail>();
    }
}
