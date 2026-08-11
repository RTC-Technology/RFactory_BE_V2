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

            // Lines arriving inside a receipt payload. The key and the foreign key stay with
            // the service: on insert the id must remain 0 for the database to generate it, and
            // on update this maps onto an entity already keyed to its receipt.
            CreateMap<GoodsReceiptLineRequest, Entities.GoodsReceiptDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.GoodsReceiptId, opt => opt.Ignore());
        }
    }
}
