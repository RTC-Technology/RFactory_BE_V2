using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Packing.DTOs;
using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Packing.Mappings
{
    public class PackingProfile:Profile
    {
        public PackingProfile()
        {
            CreateMap<Entities.PackingCheck, PackingCheckDto>();
            CreateMap<Entities.PackingCheckItem, PackingCheckItemDto>();
            CreateMap<Entities.PackingPackage, PackingPackageDto>();
            CreateMap<Entities.PackingPackageItem, PackingPackageItemDto>();
            CreateMap<Entities.PackingScanLog, PackingScanLogDto>();

            // Lines arriving inside a receipt payload. The key and the foreign key stay with
            // the service: on insert the id must remain 0 for the database to generate it, and
            // on update this maps onto an entity already keyed to its receipt.

            CreateMap<PackingCheckRequest, Entities.PackingCheck>();
            CreateMap<PackingScanLogRequest, Entities.PackingScanLog>();

            CreateMap<PackingCheckItemRequest, Entities.PackingCheckItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PackingCheckId, opt => opt.Ignore());

            CreateMap<PackingPackageRequest, Entities.PackingPackage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PackingCheckId, opt => opt.Ignore());   

            CreateMap<PackingPackageItemRequest, Entities.PackingPackageItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PackingPackageId, opt => opt.Ignore())
                .ForMember(dest => dest.PackingCheckItemId, opt => opt.Ignore());
        }
    }
}
