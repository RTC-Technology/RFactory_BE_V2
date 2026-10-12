using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class CurrencyDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Mã tiền tệ theo ISO 4217
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Tên tiền tệ
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// Ký hiệu tiền tệ
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Số chữ số thập phân
        /// </summary>
        public uint DecimalPlaces { get; set; }

        /// <summary>
        /// Có phải tiền tệ cơ sở hay không
        /// </summary>
        public bool IsBaseCurrency { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int SortOrder { get; set; }
    }
    public class CurrencyRequest
    {
        /// <summary>
        /// Mã tiền tệ theo ISO 4217
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Tên tiền tệ
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// Ký hiệu tiền tệ
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Số chữ số thập phân
        /// </summary>
        public uint DecimalPlaces { get; set; }

        /// <summary>
        /// Có phải tiền tệ cơ sở hay không
        /// </summary>
        public bool IsBaseCurrency { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int SortOrder { get; set; }
    }
}
