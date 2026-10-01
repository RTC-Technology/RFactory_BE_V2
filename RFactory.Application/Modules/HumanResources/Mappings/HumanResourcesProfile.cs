using AutoMapper;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.HumanResources.Mappings;

/// <summary>
/// AutoMapper profile for the HumanResources module. Registered by the assembly-wide
/// AutoMapper scan in <c>AddApplication</c>, so new maps here wire up automatically.
/// </summary>
public class HumanResourcesProfile : Profile
{
    public HumanResourcesProfile()
    {
        // Employee
        CreateMap<Employee, EmployeeDto>();
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<UpdateEmployeeRequest, Employee>();

        // Position
        CreateMap<Position, PositionDto>();
        CreateMap<CreatePositionRequest, Position>();
        CreateMap<UpdatePositionRequest, Position>();

        // Skill
        CreateMap<Skill, SkillDto>();
        CreateMap<CreateSkillRequest, Skill>();
        CreateMap<UpdateSkillRequest, Skill>();

        // EmployeeSkill  — request→entity only; Dto is built by the service's SQL join.
        CreateMap<CreateEmployeeSkillRequest, EmployeeSkill>();
        CreateMap<UpdateEmployeeSkillRequest, EmployeeSkill>()
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore()); // EmployeeId is immutable on update.
    }
}