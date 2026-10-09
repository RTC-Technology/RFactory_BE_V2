using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.PickingPlan.Mappings
{
    internal class PickingPlanProfile:Profile
    {
        public PickingPlanProfile()
        {
            CreateMap<Entities.PickingPlan, PickingPlanDto>();
            CreateMap<Entities.PickingPlanItem, PickingPlanItemDto>();
            CreateMap<Entities.PickingPlanItemSource, PickingPlanItemSourceDto>();
            CreateMap<Entities.PickingPlanSource, PickingPlanSourceDto>();
            CreateMap<Entities.PickingTicket, PickingTicketDto>();
            CreateMap<Entities.PickingTicketItem, PickingTicketItemDto>();

            // Lines arriving inside a receipt payload. The key and the foreign key stay with
            // the service: on insert the id must remain 0 for the database to generate it, and
            // on update this maps onto an entity already keyed to its receipt.

            CreateMap<PickingPlanRequest, Entities.PickingPlan>();

            CreateMap<PickingPlanSourceRequest, Entities.PickingPlanSource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PickingPlanId, opt => opt.Ignore());

            CreateMap<PickingPlanItemRequest, Entities.PickingPlanItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PickingPlanId, opt => opt.Ignore());

            CreateMap<PickingPlanItemSourceRequest, Entities.PickingPlanItemSource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PickingPlanItemId, opt => opt.Ignore());

            CreateMap<PickingTicketRequest, Entities.PickingTicket>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PickingPlanId, opt => opt.Ignore());

            CreateMap<PickingTicketItemRequest, Entities.PickingTicketItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PickingTicketId, opt => opt.Ignore());
        }
    }
}
