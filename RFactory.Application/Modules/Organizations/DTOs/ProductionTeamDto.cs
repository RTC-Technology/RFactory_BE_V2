using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.DTOs
{
    public class ProductionTeamDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong? CompanyId { get; set; }

        public ulong? FactoryId { get; set; }

        public ulong? WorkshopId { get; set; }

        public ulong? DepartmentId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public string ShortName { get; set; }

        public string EnglishName { get; set; }

        public ulong? TeamLeaderId { get; set; }

        public ulong? DeputyLeaderId { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
        public long? ParentId { get; set; }
    }

    public class ProductionTeamEmployeeDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong ProductionTeamId { get; set; }

        public ulong EmployeeId { get; set; }

        public bool? IsPrimary { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string? Remark { get; set; }
    }


    public class ProductionTeamRequest
    {
        public ulong? CompanyId { get; set; }

        public ulong? FactoryId { get; set; }

        public ulong? WorkshopId { get; set; }

        public ulong DepartmentId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public string? ShortName { get; set; }

        public string? EnglishName { get; set; }

        public ulong TeamLeaderId { get; set; }

        public ulong? DeputyLeaderId { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
        public long? ParentId { get; set; }
        public List<ProductionTeamEmployeeRequest>? Employees { get; set; }
    }

    public class ProductionTeamEmployeeRequest
    {
        public ulong Id { get; set; }
        public ulong? ProductionTeamId { get; set; }

        public ulong? EmployeeId { get; set; }

        public bool? IsPrimary { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string? Remark { get; set; }
    }
}
