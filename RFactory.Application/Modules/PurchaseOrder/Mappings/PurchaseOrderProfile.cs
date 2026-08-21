using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.PurchaseOrder.Mappings
{
    public class PurchaseOrderProfile:Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<Entities.PurchaseOrder, PurchaseOrderDto>();
            CreateMap<PurchaseOrderRequest, Entities.PurchaseOrder>();

            CreateMap<Entities.PurchaseOrderDetail, PurchaseOrderDetailDto>();
            CreateMap<Entities.PurchaseOrderDeliverySchedule, PurchaseOrderDeliveryScheduleDto>();
            
            CreateMap<PurchaseOrderDetailRequest, Entities.PurchaseOrderDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore());

            CreateMap<PurchaseOrderDeliveryScheduleRequest, Entities.PurchaseOrderDeliverySchedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderDetailId, opt => opt.Ignore());
        }
    }
}
