using AutoMapper;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Mappings;

public class DetailedProductionPlanProfile : Profile
{
    public DetailedProductionPlanProfile()
    {
        CreateMap<MasterProductionPlan, MasterProductionPlanDto>();
        CreateMap<MasterProductionPlanItem, MasterProductionPlanItemDto>();

        CreateMap<DetailedProductionPlan, DetailedProductionPlanDto>();
        CreateMap<DetailedProductionPlanItem, DetailedProductionPlanItemDto>();
        CreateMap<DetailedProductionPlanSchedule, DetailedProductionPlanScheduleDto>();

        CreateMap<DetailedProductionPlanItemRequest, DetailedProductionPlanItem>();
        CreateMap<DetailedProductionPlanScheduleRequest, DetailedProductionPlanSchedule>();
    }
}
