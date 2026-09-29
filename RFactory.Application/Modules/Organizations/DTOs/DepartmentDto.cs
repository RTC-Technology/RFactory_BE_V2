using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.DTOs
{
    public class DepartmentDto
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

        public ulong? ParentId { get; set; }

        public string DepartmentCode { get; set; }

        public string DepartmentName { get; set; }

        public string ShortName { get; set; }

        public string EnglishName { get; set; }

        public ulong? ManagerId { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
    }
    public class DepartmentRequest
    {

        public ulong? CompanyId { get; set; }

        public ulong? FactoryId { get; set; }

        public ulong? WorkshopId { get; set; }

        public ulong? ParentId { get; set; }

        public string DepartmentCode { get; set; }

        public string DepartmentName { get; set; }

        public string? ShortName { get; set; }

        public string? EnglishName { get; set; }

        public ulong? ManagerId { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
    }
}
