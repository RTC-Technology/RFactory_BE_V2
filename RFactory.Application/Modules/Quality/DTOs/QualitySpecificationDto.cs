using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.DTOs
{
    public class QualitySpecificationDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string SpecificationCode { get; set; }

        public string SpecificationName { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// 1: IQC (Incoming Quality Control); 2: IPQC (In-Process Quality Control); 3: OQC (Outgoing Quality Control); 4: FQC (Final Quality Control)
        /// </summary>
        public int InspectionType { get; set; }

        /// <summary>
        /// 1: Draft; 2: PendingApproval; 3: Approved; 4: Active; 5: Inactive; 6: Expired; 7: Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public string Remark { get; set; }
    }

    public class QualitySpecificationItemDto
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong QualitySpecificationId { get; set; }

        public int SequenceNo { get; set; }

        public string ParameterCode { get; set; }

        public string ParameterName { get; set; }

        public ulong? UnitId { get; set; }

        public string TargetValue { get; set; }

        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public bool? IsRequired { get; set; }

        public string Remark { get; set; }
    }

    public class QualitySpecificationProductDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong QualitySpecificationId { get; set; }

        public ulong ProductId { get; set; }
    }


    public class QualitySpecificationRequest
    {

        public string SpecificationCode { get; set; }

        public string SpecificationName { get; set; }

        public string Version { get; set; }
        public int InspectionType { get; set; }
        public int Status { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public string? Remark { get; set; }
        public List<QualitySpecificationItemRequest>? QualitySpecificationItems { get; set; }
        public List<QualitySpecificationProductRequest>? QualitySpecificationProducts { get; set; }
    }

    public class QualitySpecificationItemRequest
    {
        public ulong Id { get; set; }

        public ulong? QualitySpecificationId { get; set; }

        public int SequenceNo { get; set; }

        public string ParameterCode { get; set; }

        public string ParameterName { get; set; }

        public ulong? UnitId { get; set; }

        public string? TargetValue { get; set; }

        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public bool? IsRequired { get; set; }

        public string? Remark { get; set; }
    }

    public class QualitySpecificationProductRequest
    {
        public ulong Id { get; set; }

        public ulong? QualitySpecificationId { get; set; }

        public ulong? ProductId { get; set; }
    }
}
