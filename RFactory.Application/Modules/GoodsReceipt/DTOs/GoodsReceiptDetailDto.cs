using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.GoodsReceipt.DTOs
{
    public class GoodsReceiptDetailDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ulong GoodsReceiptId { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public ulong? LocationId { get; set; }

        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }

        public decimal Quantity { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal? UnitPrice { get; set; }

        public string? Remark { get; set; }
    }

    public class CreateGoodsReceiptDetailRequest
    {
        public ulong GoodsReceiptId { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public ulong? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Remark { get; set; }
    }

    public class UpdatesGoodsReceiptDetailRequest
    {
        public ulong Id { get; set; }
        public ulong GoodsReceiptId { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public ulong? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Remark { get; set; }
    }
}
