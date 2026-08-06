using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.MasterData.Mappings;

/// <summary>
/// AutoMapper profile for the MasterData module. Register one profile per module and
/// let <c>AddApplication</c> scan the assembly so new modules wire up automatically.
/// </summary>
public class MasterDataProfile : Profile
{
    public MasterDataProfile()
    {
        CreateMap<Factory, FactoryDto>();
        CreateMap<CreateFactoryRequest, Factory>();
        CreateMap<UpdateFactoryRequest, Factory>();

        CreateMap<Area, AreaDto>();
        CreateMap<CreateAreaRequest, Area>();
        CreateMap<UpdateAreaRequest, Area>();

        CreateMap<Line, LineDto>();
        CreateMap<CreateLineRequest, Line>();
        CreateMap<UpdateLineRequest, Line>();

        CreateMap<Organization, OrganizationDto>();
        CreateMap<CreateOrganizationRequest, Organization>();
        CreateMap<UpdateOrganizationRequest, Organization>();

        // Shift.IsActive and Shift.CrossDay are nullable bit(1) columns (bool?/ulong?) but
        // travel as plain bools, the same treatment User.IsAdmin gets.
        CreateMap<Shift, ShiftDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true))
            .ForMember(dest => dest.CrossDay, opt => opt.MapFrom(src => src.CrossDay.HasValue && src.CrossDay.Value != 0));
        CreateMap<CreateShiftRequest, Shift>()
            .ForMember(dest => dest.CrossDay, opt => opt.MapFrom(src => src.CrossDay ? 1UL : 0UL));
        CreateMap<UpdateShiftRequest, Shift>()
            .ForMember(dest => dest.CrossDay, opt => opt.MapFrom(src => src.CrossDay ? 1UL : 0UL));

        CreateMap<ShiftBreak, ShiftBreakDto>();
        CreateMap<CreateShiftBreakRequest, ShiftBreak>();
        CreateMap<UpdateShiftBreakRequest, ShiftBreak>();
    }
}
