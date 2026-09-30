using AutoMapper;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Application.Modules.Packing.DTOs;
using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Organizations.Mappings
{
    public class ProductionTeamProfile:Profile
    {
        public ProductionTeamProfile()
        {
            CreateMap<Entities.ProductionTeam, ProductionTeamDto>();
            CreateMap<Entities.ProductionTeamEmployee, ProductionTeamEmployeeDto>();

            CreateMap<ProductionTeamRequest, Entities.ProductionTeam>();
            CreateMap<ProductionTeamEmployeeRequest, Entities.ProductionTeamEmployee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionTeamId, opt => opt.Ignore());
        }
    }
}
