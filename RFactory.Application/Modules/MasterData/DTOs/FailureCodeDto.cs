using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class FailureCodeDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Mã hỏng hóc
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên hỏng hóc
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// Mô tả hỏng hóc
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Nhóm hỏng hóc
        /// </summary>
        public ulong? FailureGroupId { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng: 1-Critical, 2-Major, 3-Minor
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// Trạng thái: 1-Active, 0-Inactive
        /// </summary>
        public bool IsActive { get; set; }
    }

    public class FailureGroupDto
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
        /// Mã nhóm hỏng hóc
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// Tên nhóm hỏng hóc
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// Mô tả nhóm hỏng hóc
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Trạng thái: 1-Active, 0-Inactive
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class FailureCodeRequest
    {

        /// <summary>
        /// Mã hỏng hóc
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên hỏng hóc
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// Mô tả hỏng hóc
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Nhóm hỏng hóc
        /// </summary>
        public ulong? FailureGroupId { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng: 1-Critical, 2-Major, 3-Minor
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// Trạng thái: 1-Active, 0-Inactive
        /// </summary>
        public bool IsActive { get; set; }
    }

    public class FailureGroupRequest
    {
        
        /// <summary>
        /// Mã nhóm hỏng hóc
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// Tên nhóm hỏng hóc
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// Mô tả nhóm hỏng hóc
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Trạng thái: 1-Active, 0-Inactive
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
