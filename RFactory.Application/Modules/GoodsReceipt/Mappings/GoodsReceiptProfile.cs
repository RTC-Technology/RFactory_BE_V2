using AutoMapper;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.GoodsReceipt.Mappings
{
    public class GoodsReceiptProfile : Profile
    {
        public GoodsReceiptProfile()
        {
            CreateMap<Entities.GoodsReceipt, GoodsReceiptDto>();
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id == true));
            CreateMap<CreateGoodsReceiptRequest, Entities.GoodsReceipt>();
            CreateMap<UpdateGoodsReceiptRequest, Entities.GoodsReceipt>();


            CreateMap<Entities.GoodsReceiptDetail, GoodsReceiptDetailDto>();
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id == true));
            CreateMap<CreateGoodsReceiptDetailRequest, Entities.GoodsReceiptDetail>();
            CreateMap<UpdatesGoodsReceiptDetailRequest, Entities.GoodsReceiptDetail>();
        }
    }
}
