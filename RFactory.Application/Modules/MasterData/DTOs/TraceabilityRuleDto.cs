using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class TraceabilityRuleDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Mã quy tắc truy xuất
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// Tên quy tắc truy xuất
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng
        /// </summary>
        public ulong? ProductId { get; set; }

        /// <summary>
        /// Hướng truy xuất: 1 Forward, 2 Backward, 3 Both
        /// </summary>
        public int TraceDirection { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
    }

    public class TraceabilityRuleItemDto
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

        /// <summary>
        /// Quy tắc truy xuất
        /// </summary>
        public ulong TraceabilityRuleId { get; set; }

        /// <summary>
        /// Loại đối tượng truy xuất
        /// </summary>
        public ulong TraceTypeId { get; set; }

        /// <summary>
        /// Có bắt buộc truy xuất
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Thứ tự truy xuất
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
    }

    public class TraceabilityTypeDto
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

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Mã loại truy xuất
        /// </summary>
        public string TraceCode { get; set; }

        /// <summary>
        /// Tên loại truy xuất
        /// </summary>
        public string TraceName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
    }


    public class TraceabilityRuleRequest
    {
        /// <summary>
        /// Mã quy tắc truy xuất
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// Tên quy tắc truy xuất
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng
        /// </summary>
        public ulong? ProductId { get; set; }

        /// <summary>
        /// Hướng truy xuất: 1 Forward, 2 Backward, 3 Both
        /// </summary>
        public int TraceDirection { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
        public List<TraceabilityRuleItemRequest>? TraceabilityRuleItems { get; set; }
    }

    public class TraceabilityRuleItemRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Quy tắc truy xuất
        /// </summary>
        public ulong? TraceabilityRuleId { get; set; }

        /// <summary>
        /// Loại đối tượng truy xuất
        /// </summary>
        public ulong TraceTypeId { get; set; }

        /// <summary>
        /// Có bắt buộc truy xuất
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Thứ tự truy xuất
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
    }

    public class TraceabilityTypeRequest
    {
        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Mã loại truy xuất
        /// </summary>
        public string TraceCode { get; set; }

        /// <summary>
        /// Tên loại truy xuất
        /// </summary>
        public string TraceName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
    }
}
