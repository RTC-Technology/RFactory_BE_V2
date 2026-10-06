using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class LotDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string LotNo { get; set; }

        /// <summary>
        /// Quy tắc Lot được sử dụng để tạo Lot
        /// </summary>
        public ulong? LotRuleId { get; set; }

        public ulong ProductId { get; set; }

        public ulong? SupplierId { get; set; }

        public string SupplierLotNo { get; set; }

        public DateOnly? ManufactureDate { get; set; }

        public DateOnly? ExpireDate { get; set; }

        /// <summary>
        /// 1: AVAILABLE;2: HOLD; 3: BLOCKED; 4: CLOSED
        /// </summary>
        public int? Status { get; set; }
    }

    public class LotRuleDto
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
        /// Mã quy tắc Lot
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// Tên quy tắc Lot
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Mẫu sinh mã Lot
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Tiền tố Lot
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Định dạng ngày
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Độ dài số thứ tự
        /// </summary>
        public int? SequenceLength { get; set; }

        /// <summary>
        /// Cách reset sequence: 1 Daily; 2 Monthly; 3 Yearly; 4 Never
        /// </summary>
        public int SequenceResetType { get; set; }

        /// <summary>
        /// Trạng thái: 1 Draft; 2 Active; 3 Inactive
        /// </summary>
        public int Status { get; set; }

        public string Description { get; set; }
    }

    public class LotRuleSequenceDto
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
        /// Quy tắc Lot
        /// </summary>
        public ulong LotRuleId { get; set; }

        /// <summary>
        /// Khóa phạm vi sequence
        /// </summary>
        public string SequenceKey { get; set; }

        /// <summary>
        /// Giá trị sequence hiện tại
        /// </summary>
        public ulong CurrentValue { get; set; }
    }

    public class ProductLotRuleDto
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
        /// Sản phẩm
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Quy tắc Lot
        /// </summary>
        public ulong LotRuleId { get; set; }

        /// <summary>
        /// Có phải quy tắc mặc định hay không
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// Ngày bắt đầu áp dụng
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// Ngày kết thúc áp dụng
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
    }


    public class LotRequest
    {
        
        public string LotNo { get; set; }

        /// <summary>
        /// Quy tắc Lot được sử dụng để tạo Lot
        /// </summary>
        public ulong? LotRuleId { get; set; }

        public ulong ProductId { get; set; }

        public ulong? SupplierId { get; set; }

        public string SupplierLotNo { get; set; }

        public DateOnly? ManufactureDate { get; set; }

        public DateOnly? ExpireDate { get; set; }

        /// <summary>
        /// 1: AVAILABLE;2: HOLD; 3: BLOCKED; 4: CLOSED
        /// </summary>
        public int? Status { get; set; }
    }

    public class LotRuleRequest
    {
        
        public string RuleCode { get; set; }

        /// <summary>
        /// Tên quy tắc Lot
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Mẫu sinh mã Lot
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Tiền tố Lot
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Định dạng ngày
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Độ dài số thứ tự
        /// </summary>
        public int? SequenceLength { get; set; }

        /// <summary>
        /// Cách reset sequence: 1 Daily; 2 Monthly; 3 Yearly; 4 Never
        /// </summary>
        public int SequenceResetType { get; set; }

        /// <summary>
        /// Trạng thái: 1 Draft; 2 Active; 3 Inactive
        /// </summary>
        public int Status { get; set; }

        public string? Description { get; set; }
        public List<ProductLotRuleRequest>? ProductLotRules { get; set; }
    }

    public class LotRuleSequenceRequest
    {
       
        /// <summary>
        /// Quy tắc Lot
        /// </summary>
        public ulong LotRuleId { get; set; }

        /// <summary>
        /// Khóa phạm vi sequence
        /// </summary>
        public string SequenceKey { get; set; }

        /// <summary>
        /// Giá trị sequence hiện tại
        /// </summary>
        public ulong CurrentValue { get; set; }
    }

    public class ProductLotRuleRequest
    {
        
        public ulong Id { get; set; }

        /// <summary>
        /// Sản phẩm
        /// </summary>
        public ulong? ProductId { get; set; }

        /// <summary>
        /// Quy tắc Lot
        /// </summary>
        public ulong? LotRuleId { get; set; }

        /// <summary>
        /// Có phải quy tắc mặc định hay không
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// Ngày bắt đầu áp dụng
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// Ngày kết thúc áp dụng
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
    }
}
