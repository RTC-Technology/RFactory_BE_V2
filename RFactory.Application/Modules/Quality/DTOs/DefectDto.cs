using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.DTOs
{
    public class DefectDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong DefectGroupId { get; set; }

        public string DefectCode { get; set; }

        public string DefectName { get; set; }

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int Severity { get; set; }

        public int SortOrder { get; set; }

        public bool? IsActive { get; set; }
    }

    public partial class DefectGroupDto
    {

        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool? IsActive { get; set; }
    }


    public class DefectRequest
    {

        public ulong DefectGroupId { get; set; }

        public string DefectCode { get; set; }

        public string DefectName { get; set; }

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int Severity { get; set; }

        public int SortOrder { get; set; }

        public bool? IsActive { get; set; }
    }

    public partial class DefectGroupRequest
    {

        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
