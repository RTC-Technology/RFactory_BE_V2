using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class SerialDto
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
        /// Mã Serial duy nhất
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// Sản phẩm của Serial
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Lô hàng của Serial
        /// </summary>
        public ulong? LotId { get; set; }

        /// <summary>
        /// Quy tắc Serial đã sử dụng
        /// </summary>
        public ulong? SerialRuleId { get; set; }

        /// <summary>
        /// Sequence record đã cấp số
        /// </summary>
        public ulong? SerialRuleSequenceId { get; set; }

        /// <summary>
        /// Số thứ tự được cấp
        /// </summary>
        public ulong? SequenceNo { get; set; }

        /// <summary>
        /// Trạng thái Serial: 1 Available, 2 Used, 3 Hold, 4 Blocked, 5 Scrapped
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Ngày sản xuất
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// Ngày kích hoạt Serial
        /// </summary>
        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Remark { get; set; }
    }

    public class SerialRuleDto
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
        /// Mã quy tắc Serial
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// Tên quy tắc Serial
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng quy tắc
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Tiền tố Serial
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Hậu tố Serial
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Định dạng ngày, ví dụ yyyyMMdd
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Ký tự phân cách các thành phần
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Số chữ số của sequence
        /// </summary>
        public int SequenceLength { get; set; }

        /// <summary>
        /// Kiểu reset: 1 Never, 2 Daily, 3 Monthly, 4 Yearly
        /// </summary>
        public int SequenceResetType { get; set; }

        /// <summary>
        /// Mẫu sinh Serial, ví dụ {PREFIX}{SEP}{DATE}{SEP}{SEQ}
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Trạng thái: 1 Active, 2 Inactive
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Quy tắc mặc định của sản phẩm
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Remark { get; set; }
    }

    public class SerialRuleSequenceDto
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
        /// ID quy tắc Serial
        /// </summary>
        public ulong SerialRuleId { get; set; }

        /// <summary>
        /// Kỳ sequence, ví dụ 20261006, 202610, 2026 hoặc GLOBAL
        /// </summary>
        public string SequencePeriod { get; set; }

        /// <summary>
        /// Sequence hiện tại
        /// </summary>
        public ulong CurrentSequence { get; set; }
    }

    public class SerialRequest
    {

        /// <summary>
        /// Mã Serial duy nhất
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// Sản phẩm của Serial
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Lô hàng của Serial
        /// </summary>
        public ulong? LotId { get; set; }

        /// <summary>
        /// Quy tắc Serial đã sử dụng
        /// </summary>
        public ulong? SerialRuleId { get; set; }

        /// <summary>
        /// Sequence record đã cấp số
        /// </summary>
        public ulong? SerialRuleSequenceId { get; set; }

        /// <summary>
        /// Số thứ tự được cấp
        /// </summary>
        public ulong? SequenceNo { get; set; }

        /// <summary>
        /// Trạng thái Serial: 1 Available, 2 Used, 3 Hold, 4 Blocked, 5 Scrapped
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Ngày sản xuất
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// Ngày kích hoạt Serial
        /// </summary>
        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Remark { get; set; }
    }

    public class SerialRuleRequest
    {

        /// <summary>
        /// Mã quy tắc Serial
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// Tên quy tắc Serial
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Sản phẩm áp dụng quy tắc
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Tiền tố Serial
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Hậu tố Serial
        /// </summary>
        public string? Suffix { get; set; }

        /// <summary>
        /// Định dạng ngày, ví dụ yyyyMMdd
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Ký tự phân cách các thành phần
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Số chữ số của sequence
        /// </summary>
        public int SequenceLength { get; set; }

        /// <summary>
        /// Kiểu reset: 1 Never, 2 Daily, 3 Monthly, 4 Yearly
        /// </summary>
        public int SequenceResetType { get; set; }

        /// <summary>
        /// Mẫu sinh Serial, ví dụ {PREFIX}{SEP}{DATE}{SEP}{SEQ}
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Trạng thái: 1 Active, 2 Inactive
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Quy tắc mặc định của sản phẩm
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Remark { get; set; }
    }

    public class SerialRuleSequenceRequest
    {

        /// <summary>
        /// ID quy tắc Serial
        /// </summary>
        public ulong SerialRuleId { get; set; }

        /// <summary>
        /// Kỳ sequence, ví dụ 20261006, 202610, 2026 hoặc GLOBAL
        /// </summary>
        public string SequencePeriod { get; set; }

        /// <summary>
        /// Sequence hiện tại
        /// </summary>
        public ulong CurrentSequence { get; set; }
    }
}
