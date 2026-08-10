using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.GoodsReceipt.DTOs
{
    public class GoodsReceiptDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public string ReceiptNo { get; set; } = string.Empty;
        public ulong WarehouseId { get; set; }
        public ulong? SupplierId { get; set; }
        public string? ReferenceType { get; set; }
        public ulong? ReferenceId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? Remark { get; set; }

        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ulong? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }

        public int ReceiptType { get; set; }
    }

    public class CreateGoodsReceiptRequest
    {
        public string ReceiptNo { get; set; } = string.Empty;
        public ulong WarehouseId { get; set; }
        public ulong? SupplierId { get; set; }
        public string? ReferenceType { get; set; }
        public ulong? ReferenceId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? Remark { get; set; }
        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ulong? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public int ReceiptType { get; set; }
        public List<GoodsReceiptDetailDto> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetailDto>();
    }

    public class UpdateGoodsReceiptRequest
    {
        public string ReceiptNo { get; set; } = string.Empty;
        public ulong WarehouseId { get; set; }
        public ulong? SupplierId { get; set; }
        public string? ReferenceType { get; set; }
        public ulong? ReferenceId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? Remark { get; set; }
        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ulong? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public int ReceiptType { get; set; }
        public List<GoodsReceiptDetailDto> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetailDto>();
    }
}
