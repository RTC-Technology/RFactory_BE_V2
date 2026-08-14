using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.GoodsIssue.Mappings;
public class GoodsIssueProfile : Profile
{
    public GoodsIssueProfile()
    {
        CreateMap<Entities.GoodsIssue, GoodsIssueDto>();
        //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id == true));
        CreateMap<CreateGoodsIssueRequest, Entities.GoodsIssue>();
        CreateMap<UpdateGoodsIssueRequest, Entities.GoodsIssue>();


        CreateMap<Entities.GoodsIssueDetail, GoodsIssueDetailDto>();
        //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id == true));
        //CreateMap<CreateGoodsReceiptDetailRequest, Entities.GoodsReceiptDetail>();
        //CreateMap<UpdatesGoodsReceiptDetailRequest, Entities.GoodsReceiptDetail>();

        // Lines arriving inside a receipt payload. The key and the foreign key stay with
        // the service: on insert the id must remain 0 for the database to generate it, and
        // on update this maps onto an entity already keyed to its receipt.
        CreateMap<GoodsIssueDetailRequest, Entities.GoodsIssueDetail>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GoodsIssueId, opt => opt.Ignore());
    }
}
