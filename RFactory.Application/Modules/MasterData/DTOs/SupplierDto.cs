using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class SupplierDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? TaxCode { get; set; }
        public string? SupplierType { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? ContactPerson { get; set; }
        public string? PaymentTerm { get; set; }
        public string? CurrencyCode { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
    }

    public class CreateSupplierRequest
    {
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? TaxCode { get; set; }
        public string? SupplierType { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? ContactPerson { get; set; }
        public string? PaymentTerm { get; set; }
        public string? CurrencyCode { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateSupplierRequest
    {
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? TaxCode { get; set; }
        public string? SupplierType { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? ContactPerson { get; set; }
        public string? PaymentTerm { get; set; }
        public string? CurrencyCode { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
    }
}
