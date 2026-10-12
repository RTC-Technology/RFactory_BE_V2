using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.DeliveryNote.Mappings
{
    public class DeliveryNoteProfile : Profile
    {
        public DeliveryNoteProfile()
        {
            CreateMap<Entities.DeliveryNote, DeliveryNoteDto>();
            CreateMap<Entities.DeliveryNoteItem, DeliveryNoteItemDto>();
            CreateMap<Entities.DeliveryNoteSender, DeliveryNoteSenderDto>();
            CreateMap<Entities.DeliveryNoteSource, DeliveryNoteSourceDto>();
            CreateMap<Entities.DeliveryNoteReceiver, DeliveryNoteReceiverDto>();

            // Lines arriving inside a receipt payload. The key and the foreign key stay with
            // the service: on insert the id must remain 0 for the database to generate it, and
            // on update this maps onto an entity already keyed to its receipt.

            CreateMap<DeliveryNoteRequest, Entities.DeliveryNote>();

            CreateMap<DeliveryNoteItemRequest, Entities.DeliveryNoteItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryNoteId, opt => opt.Ignore());

            CreateMap<DeliveryNoteSenderRequest, Entities.DeliveryNoteSender>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryNoteId, opt => opt.Ignore());

            CreateMap<DeliveryNoteSourceRequest, Entities.DeliveryNoteSource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryNoteId, opt => opt.Ignore());

            CreateMap<DeliveryNoteReceiverRequest, Entities.DeliveryNoteReceiver>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryNoteId, opt => opt.Ignore());
        }
    }
}
