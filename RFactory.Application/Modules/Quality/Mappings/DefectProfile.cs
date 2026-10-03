using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Quality.Mappings
{
    public class DefectProfile:Profile
    {
        public DefectProfile()
        {
            CreateMap<Entities.Defect, DTOs.DefectDto>();
            CreateMap<Entities.DefectGroup, DTOs.DefectGroupDto>();

            CreateMap<DTOs.DefectRequest, Entities.Defect>();
            CreateMap<DTOs.DefectGroupRequest, Entities.DefectGroup>();
        }
    }
}
