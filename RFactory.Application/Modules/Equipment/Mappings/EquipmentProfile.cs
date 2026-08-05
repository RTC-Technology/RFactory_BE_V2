using AutoMapper;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Equipment.Mappings;

/// <summary>
/// AutoMapper profile for the Equipment module (machine types, machines).
/// </summary>
public class EquipmentProfile : Profile
{
    public EquipmentProfile()
    {
        CreateMap<MachineType, MachineTypeDto>();
        CreateMap<CreateMachineTypeRequest, MachineType>();
        CreateMap<UpdateMachineTypeRequest, MachineType>();

        CreateMap<Machine, MachineDto>();
        CreateMap<CreateMachineRequest, Machine>();
        CreateMap<UpdateMachineRequest, Machine>();
    }
}
