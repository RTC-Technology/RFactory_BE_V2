using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.DTOs
{
    public class SamplingPlanDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string SamplingPlanCode { get; set; }

        public string SamplingPlanName { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// 1 :FixedQuantity; 2 : Percentage; 3 : LotSizeBased; 4 : AQL; 5 : 100Percent
        /// </summary>
        public int? SamplingMethod { get; set; }

        /// <summary>
        /// Level I: mức kiểm tra thấp hơn; 
        /// Level II: mức kiểm tra thông thường
        /// Level; III: mức kiểm tra cao hơn
        /// </summary>
        public string? InspectionLevel { get; set; }

        /// <summary>
        /// Acceptable Quality Limit
        /// </summary>
        public decimal? AqlValue { get; set; }

        /// <summary>
        /// 1 = PerLot; 2 = PerShift;3 = PerHour; 4 = PerDay; 5 = PerQuantity; 6 = FirstPiece; 7 = LastPiece; 8 = Periodic
        /// </summary>
        public int? FrequencyType { get; set; }

        public decimal? FrequencyValue { get; set; }

        /// <summary>
        /// 1 = Draft; 2 = PendingApproval; 3 = Approved; 4 = Active; 5 = Inactive; 6 = Expired; 7 = Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public string? Remark { get; set; }
    }

    public class SamplingPlanRuleDto
    {

        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public ulong? SamplingPlanId { get; set; }

        public ulong? LotSizeFrom { get; set; }

        public ulong? LotSizeTo { get; set; }

        public ulong? SampleSize { get; set; }

        public int? AcceptanceNumber { get; set; }

        public int? RejectionNumber { get; set; }

        public int SortOrder { get; set; }
    }

    public class SamplingPlanRequest
    {
        public string SamplingPlanCode { get; set; }

        public string SamplingPlanName { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// 1 :FixedQuantity; 2 : Percentage; 3 : LotSizeBased; 4 : AQL; 5 : 100Percent
        /// </summary>
        public int? SamplingMethod { get; set; }

        /// <summary>
        /// Level I: mức kiểm tra thấp hơn; 
        /// Level II: mức kiểm tra thông thường
        /// Level; III: mức kiểm tra cao hơn
        /// </summary>
        public string? InspectionLevel { get; set; }

        /// <summary>
        /// Acceptable Quality Limit
        /// </summary>
        public decimal? AqlValue { get; set; }

        /// <summary>
        /// 1 = PerLot; 2 = PerShift;3 = PerHour; 4 = PerDay; 5 = PerQuantity; 6 = FirstPiece; 7 = LastPiece; 8 = Periodic
        /// </summary>
        public int? FrequencyType { get; set; }

        public decimal? FrequencyValue { get; set; }

        /// <summary>
        /// 1 = Draft; 2 = PendingApproval; 3 = Approved; 4 = Active; 5 = Inactive; 6 = Expired; 7 = Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public string? Remark { get; set; }
        public List<SamplingPlanRuleRequest>? SamplingPlanRules { get; set; }
    }

    public class SamplingPlanRuleRequest
    {

        public ulong Id { get; set; }

        public ulong? SamplingPlanId { get; set; }

        public ulong? LotSizeFrom { get; set; }

        public ulong? LotSizeTo { get; set; }

        public ulong? SampleSize { get; set; }

        public int? AcceptanceNumber { get; set; }

        public int? RejectionNumber { get; set; }

        public int SortOrder { get; set; }
    }
}
