using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.DTOs
{
    public class WorkCenterDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong? FactoryId { get; set; }

        public ulong? WorkshopId { get; set; }

        public string WorkCenterCode { get; set; }

        public string WorkCenterName { get; set; }

        public string ShortName { get; set; }

        public string EnglishName { get; set; }

        /// <summary>
        /// 1: Production; 2: Assembly; 3: Inspection; 4: Packaging; 5: Warehouse; 6: Maintenance; 99: Other
        /// </summary>
        public int? WorkCenterType { get; set; }

        public ulong? ManagerId { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 1: Active; 2: Inactive;3: Maintenance
        /// </summary>
        public int Status { get; set; }

        public int SortOrder { get; set; }
    }
    public class WorkCenterRequest
    {

        public ulong? FactoryId { get; set; }

        public ulong? WorkshopId { get; set; }

        public string WorkCenterCode { get; set; }

        public string WorkCenterName { get; set; }

        public string? ShortName { get; set; }

        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Production; 2: Assembly; 3: Inspection; 4: Packaging; 5: Warehouse; 6: Maintenance; 99: Other
        /// </summary>
        public int? WorkCenterType { get; set; }

        public ulong ManagerId { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// 1: Active; 2: Inactive;3: Maintenance
        /// </summary>
        public int Status { get; set; }

        public int SortOrder { get; set; }
    }
}
