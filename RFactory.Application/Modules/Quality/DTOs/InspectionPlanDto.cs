using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.DTOs
{
    public class InspectionPlanDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string PlanCode { get; set; }

        public string PlanName { get; set; }

        public ulong ProductId { get; set; }

        public ulong RoutingId { get; set; }

        public ulong QualitySpecificationId { get; set; }

        /// <summary>
        /// 1=IQC, 2=IPQC, 3=OQC, 4=FQC
        /// </summary>
        public int InspectionType { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// 1=Draft, 2=PendingApproval, 3=Approved, 4=Active, 5=Inactive, 6=Expired, 7=Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public ulong? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? Remark { get; set; }
    }
    public partial class InspectionItemDto
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

        public ulong? InspectionPlanId { get; set; }

        public ulong QualitySpecificationItemId { get; set; }

        public int SequenceNo { get; set; }

        public string InspectionMethod { get; set; }

        public decimal? SampleSize { get; set; }

        public string Frequency { get; set; }

        public bool IsRequired { get; set; }

        public string? Remark { get; set; }
    }

    public partial class InspectionExecutionDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string ExecutionNo { get; set; }

        public ulong InspectionPlanId { get; set; }

        public ulong ProductId { get; set; }

        public string? LotNo { get; set; }

        public string? SerialNo { get; set; }

        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        public ulong? ProductionOrderId { get; set; }

        public int InspectionType { get; set; }

        /// <summary>
        /// 1=Pending, 2=InProgress, 3=Passed, 4=Failed, 5=PartiallyPassed, 6=Cancelled
        /// </summary>
        public int Status { get; set; }

        public decimal? SampleSize { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ulong? InspectorId { get; set; }

        public string? Remark { get; set; }
    }

    public partial class InspectionResultDto
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

        public ulong InspectionExecutionId { get; set; }

        public ulong InspectionItemId { get; set; }

        public int? SampleNo { get; set; }

        public string ActualValue { get; set; }

        public decimal? NumericValue { get; set; }

        public string? TextValue { get; set; }

        public bool? BooleanValue { get; set; }

        /// <summary>
        /// 1=Pending, 2=Pass, 3=Fail, 4=NA
        /// </summary>
        public int Result { get; set; }

        public DateTime? InspectionTime { get; set; }

        public ulong? InspectorId { get; set; }

        public string? Remark { get; set; }
    }


    public class InspectionPlanRequest
    {

        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public ulong ProductId { get; set; }
        public ulong RoutingId { get; set; }
        public ulong QualitySpecificationId { get; set; }
        /// <summary>
        /// 1=IQC, 2=IPQC, 3=OQC, 4=FQC
        /// </summary>
        public int InspectionType { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// 1=Draft, 2=PendingApproval, 3=Approved, 4=Active, 5=Inactive, 6=Expired, 7=Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public ulong? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? Remark { get; set; }
        public List<InspectionItemRequest>? InspectionItems { get; set; }
    }

    public partial class InspectionItemRequest
    {
       
        public ulong Id { get; set; }

        public ulong? InspectionPlanId { get; set; }

        public ulong QualitySpecificationItemId { get; set; }

        public int SequenceNo { get; set; }

        public string InspectionMethod { get; set; }

        public decimal? SampleSize { get; set; }

        public string Frequency { get; set; }

        public bool IsRequired { get; set; }

        public string? Remark { get; set; }
    }

    public partial class InspectionExecutionRequest
    {
        public string ExecutionNo { get; set; }

        public ulong? InspectionPlanId { get; set; }

        public ulong ProductId { get; set; }

        public string? LotNo { get; set; }

        public string? SerialNo { get; set; }

        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        public ulong? ProductionOrderId { get; set; }

        public int InspectionType { get; set; }

        /// <summary>
        /// 1=Pending, 2=InProgress, 3=Passed, 4=Failed, 5=PartiallyPassed, 6=Cancelled
        /// </summary>
        public int Status { get; set; }

        public decimal? SampleSize { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ulong? InspectorId { get; set; }

        public string? Remark { get; set; }
        public List<InspectionResultRequest>? InspectionResults { get; set; }
    }

    public partial class InspectionResultRequest
    {
        public ulong Id { get; set; }

        public ulong? InspectionExecutionId { get; set; }

        public ulong? InspectionItemId { get; set; }

        public int? SampleNo { get; set; }

        public string ActualValue { get; set; }

        public decimal? NumericValue { get; set; }

        public string? TextValue { get; set; }

        public bool? BooleanValue { get; set; }

        /// <summary>
        /// 1=Pending, 2=Pass, 3=Fail, 4=NA
        /// </summary>
        public int Result { get; set; }

        public DateTime? InspectionTime { get; set; }

        public ulong? InspectorId { get; set; }

        public string? Remark { get; set; }
    }
}
