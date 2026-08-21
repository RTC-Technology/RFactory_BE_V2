using RFactory.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.PurchaseOrder.DTOs
{
    public class PurchaseOrderDto
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string Pono { get; set; } = string.Empty;
        public ulong SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        /// <summary>
        /// 1: Draft; 2: Approved; 3: PartiallyReceived; 4: FullyReceived; 5: Cancelled; 6: Closed
        /// </summary>
        public int? Status { get; set; }
    }

    public class PurchaseOrderRequest
    {
        public ulong Id { get; set; }
        public string Pono { get; set; } = string.Empty;
        public ulong SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        /// <summary>
        /// 1: Draft; 2: Approved; 3: PartiallyReceived; 4: FullyReceived; 5: Cancelled; 6: Closed
        /// </summary>
        public int? Status { get; set; }
        public List<PurchaseOrderDetailRequest>? PurchaseOrderDetailRequests { get; set; }
    }

    public class PurchaseOrderDetailDto
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ulong PurchaseOrderId { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class PurchaseOrderDetailRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }
        public ulong PurchaseOrderId { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public List<PurchaseOrderDeliveryScheduleRequest>? PurchaseOrderDeliveryScheduleRequests { get; set; }
    }

    public class PurchaseOrderDeliveryScheduleDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ulong PurchaseOrderDetailId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Quantity { get; set; }
    }

    public class PurchaseOrderDeliveryScheduleRequest
    {
        public ulong Id { get; set; }
        public ulong PurchaseOrderDetailId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Quantity { get; set; }
    }

    /// <summary>
    /// Trạng thái Purchase Order
    /// </summary>
    public enum PurchaseOrderStatus
    {
        /// <summary>Nháp</summary>
        Draft = 1,

        /// <summary>Đã duyệt</summary>
        Approved = 2,

        /// <summary>Đã nhận một phần</summary>
        PartiallyReceived = 3,

        /// <summary>Đã nhận đủ</summary>
        FullyReceived = 4,

        /// <summary>Đã hủy</summary>
        Cancelled = 5,

        /// <summary>Đã đóng</summary>
        Closed = 6
    }
}
