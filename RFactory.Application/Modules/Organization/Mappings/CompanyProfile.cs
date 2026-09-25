using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Organizations.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Organizations.Mappings
{
    public class CompanyProfile:Profile
    {
        public CompanyProfile()
        {
            CreateMap<Entities.Company, CompanyDto>();
            CreateMap<CompanyRequest, Entities.Company>();
            
        }
    }
}
