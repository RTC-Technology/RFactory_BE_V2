using AutoMapper;
using RFactory.Application.Modules.Production.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Mappings
{
    public class ProductionRequestProfile:Profile
    {
        public ProductionRequestProfile()
        {
            CreateMap<Entities.ProductionRequest, ProductionRequestDto>();
            CreateMap<Entities.ProductionRequestItem, ProductionRequestItemDto>();

            CreateMap<ProductionRequestRequest, Entities.ProductionRequest>();
            CreateMap<ProductionRequestItemRequest, Entities.ProductionRequestItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionRequestId, opt => opt.Ignore());

        }
    }
}
