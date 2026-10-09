using AutoMapper;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Quality.Mappings
{
    public class QualitySpecificationProfile:Profile
    {
        public QualitySpecificationProfile()
        {
            CreateMap<Entities.QualitySpecification, QualitySpecificationDto>();
            CreateMap<Entities.QualitySpecificationItem, QualitySpecificationItemDto>();
            CreateMap<Entities.QualitySpecificationProduct, QualitySpecificationProductDto>();

            CreateMap<QualitySpecificationRequest, Entities.QualitySpecification>();
            CreateMap<QualitySpecificationItemRequest, Entities.QualitySpecificationItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QualitySpecificationId, opt => opt.Ignore());

            CreateMap<QualitySpecificationProductRequest, Entities.QualitySpecificationProduct>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QualitySpecificationId, opt => opt.Ignore());
        }
    }
}
