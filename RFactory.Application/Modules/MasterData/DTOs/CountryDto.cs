using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class CountryDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Mã quốc gia, ví dụ VN, US
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Tên quốc gia
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// Mã điện thoại quốc tế
        /// </summary>
        public string? PhoneCode { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class ProvinceDto
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
        /// Quốc gia
        /// </summary>
        public ulong? CountryId { get; set; }

        /// <summary>
        /// Mã tỉnh/thành phố
        /// </summary>
        public string ProvinceCode { get; set; }

        /// <summary>
        /// Tên tỉnh/thành phố
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Tỉnh, 2: Thành phố trực thuộc trung ương
        /// </summary>
        public int ProvinceType { get; set; }

        /// <summary>
        /// Mã bưu chính
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class DistrictDto
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
        /// Tỉnh/thành phố
        /// </summary>
        public ulong? ProvinceId { get; set; }

        /// <summary>
        /// Mã quận/huyện
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Quận, 2: Huyện, 3: Thị xã, 4: Thành phố thuộc tỉnh
        /// </summary>
        public int DistrictType { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public partial class WardDto
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
        /// Quận/huyện
        /// </summary>
        public ulong? DistrictId { get; set; }

        /// <summary>
        /// Mã xã/phường
        /// </summary>
        public string WardCode { get; set; }

        /// <summary>
        /// Tên xã/phường
        /// </summary>
        public string WardName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Phường, 2: Xã, 3: Thị trấn
        /// </summary>
        public byte WardType { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class CountryRequest
    {
        
        /// <summary>
        /// Mã quốc gia, ví dụ VN, US
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Tên quốc gia
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// Mã điện thoại quốc tế
        /// </summary>
        public string? PhoneCode { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class ProvinceRequest
    {
        public ulong? CountryId { get; set; }

        /// <summary>
        /// Mã tỉnh/thành phố
        /// </summary>
        public string ProvinceCode { get; set; }

        /// <summary>
        /// Tên tỉnh/thành phố
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Tỉnh, 2: Thành phố trực thuộc trung ương
        /// </summary>
        public int ProvinceType { get; set; }

        /// <summary>
        /// Mã bưu chính
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class DistrictRequest
    {

        /// <summary>
        /// Tỉnh/thành phố
        /// </summary>
        public ulong? ProvinceId { get; set; }

        /// <summary>
        /// Mã quận/huyện
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Quận, 2: Huyện, 3: Thị xã, 4: Thành phố thuộc tỉnh
        /// </summary>
        public int DistrictType { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class WardRequest
    {
        /// <summary>
        /// Quận/huyện
        /// </summary>
        public ulong? DistrictId { get; set; }

        /// <summary>
        /// Mã xã/phường
        /// </summary>
        public string WardCode { get; set; }

        /// <summary>
        /// Tên xã/phường
        /// </summary>
        public string WardName { get; set; }

        /// <summary>
        /// Tên tiếng Anh
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 1: Phường, 2: Xã, 3: Thị trấn
        /// </summary>
        public byte WardType { get; set; }

        /// <summary>
        /// 1: Hoạt động, 0: Ngừng sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
