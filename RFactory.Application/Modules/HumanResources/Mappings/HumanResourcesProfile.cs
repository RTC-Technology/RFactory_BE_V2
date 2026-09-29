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
        CreateMap<Employee, EmployeeDto>();
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<UpdateEmployeeRequest, Employee>();
    }
}