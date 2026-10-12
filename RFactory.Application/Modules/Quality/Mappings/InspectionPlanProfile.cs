using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Quality.Mappings
{
    public class InspectionPlanProfile:Profile
    {
        public InspectionPlanProfile()
        {
            CreateMap<Entities.InspectionPlan, DTOs.InspectionPlanDto>();
            CreateMap<Entities.InspectionItem, DTOs.InspectionItemDto>();
            CreateMap<Entities.InspectionExecution, DTOs.InspectionExecutionDto>();
            CreateMap<Entities.InspectionResult, DTOs.InspectionResultDto>();

            CreateMap<DTOs.InspectionPlanRequest, Entities.InspectionPlan>();
            CreateMap<DTOs.InspectionItemRequest, Entities.InspectionItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InspectionPlanId, opt => opt.Ignore());

            CreateMap<DTOs.InspectionExecutionRequest, Entities.InspectionExecution>();

            CreateMap<DTOs.InspectionResultRequest, Entities.InspectionResult>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InspectionExecutionId, opt => opt.Ignore());
        }
    }
}
