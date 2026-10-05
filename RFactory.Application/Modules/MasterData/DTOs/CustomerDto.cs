using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class CustomerDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShortName { get; set; }
        public string? EnglishName { get; set; }
        public int CustomerType { get; set; }
        public string? TaxCode { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public long? CountryId { get; set; }
        public ulong? ProvinceId { get; set; }
        public ulong? DistrictId { get; set; }
        public string? PaymentTerm { get; set; }
        public ulong? CurrencyId { get; set; }
        public ulong? DefaultWarehouseId { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CustomerContactDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public ulong CustomerId { get; set; }
        public string? ContactName { get; set; }
        public int ContactType { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsPrimary { get; set; }
        public bool? IsActive { get; set; }
        public string? Remark { get; set; }
    }

    public class CustomerRequest
    {
    
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShortName { get; set; }
        public string? EnglishName { get; set; }
        public int CustomerType { get; set; }
        public string? TaxCode { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public long? CountryId { get; set; }
        public ulong? ProvinceId { get; set; }
        public ulong? DistrictId { get; set; }
        public string? PaymentTerm { get; set; }
        public ulong? CurrencyId { get; set; }
        public ulong? DefaultWarehouseId { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
        public List<CustomerContactRequest>? CustomerContacts { get; set; }
    }

    public class CustomerContactRequest
    {
        public ulong Id { get; set; }
        public ulong? CustomerId { get; set; }
        public string ContactName { get; set; }
        public int ContactType { get; set; }
        public string? Position { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public bool IsPrimary { get; set; }
        public bool? IsActive { get; set; }
        public string? Remark { get; set; }
    }
}
