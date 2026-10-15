using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Mappings
{
    public class SalesOrderProfile:Profile
    {
        public SalesOrderProfile()
        {
            CreateMap<Entities.SalesOrder, SalesOrderDto>();
            CreateMap<Entities.SalesOrderItem, SalesOrderItemDto>();
            CreateMap<Entities.SalesOrderPaymentSchedule, SalesOrderPaymentScheduleDto>();


            CreateMap<SalesOrderRequest, Entities.SalesOrder>();

            CreateMap<SalesOrderItemRequest, Entities.SalesOrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SalesOrderId, opt => opt.Ignore());

            CreateMap<SalesOrderPaymentScheduleRequest, Entities.SalesOrderPaymentSchedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SalesOrderId, opt => opt.Ignore());

        }
    }
}
