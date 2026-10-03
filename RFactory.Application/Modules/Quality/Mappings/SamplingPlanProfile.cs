using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Quality.Mappings
{
    public class SamplingPlanProfile:Profile
    {
        public SamplingPlanProfile()
        {
            CreateMap<Entities.SamplingPlan, DTOs.SamplingPlanDto>();
            CreateMap<Entities.SamplingPlanRule, DTOs.SamplingPlanRuleDto>();

            CreateMap<DTOs.SamplingPlanRequest, Entities.SamplingPlan>();
            CreateMap<DTOs.SamplingPlanRuleRequest, Entities.SamplingPlanRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SamplingPlanId, opt => opt.Ignore());
        }
    }
}
