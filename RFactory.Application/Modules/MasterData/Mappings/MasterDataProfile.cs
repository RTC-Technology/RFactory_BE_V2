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
    }
}
