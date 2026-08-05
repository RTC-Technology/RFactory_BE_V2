using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Administration.Mappings;

/// <summary>
/// AutoMapper profile for the Administration module (users, menus). <see cref="User.IsAdmin"/>
/// is stored as a nullable bit(1) column (ulong?) but exposed as a plain bool in the DTOs.
/// </summary>
public class AdministrationProfile : Profile
{
    public AdministrationProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin.HasValue && src.IsAdmin.Value != 0));

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin ? 1UL : 0UL));

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin ? 1UL : 0UL));

        CreateMap<Menu, MenuDto>();
        CreateMap<CreateMenuRequest, Menu>();
        CreateMap<UpdateMenuRequest, Menu>();

        CreateMap<FunctionGroup, FunctionGroupDto>();
        CreateMap<CreateFunctionGroupRequest, FunctionGroup>();
        CreateMap<UpdateFunctionGroupRequest, FunctionGroup>();

        CreateMap<Function, FunctionDto>();
        CreateMap<CreateFunctionRequest, Function>();
        CreateMap<UpdateFunctionRequest, Function>();
    }
}
