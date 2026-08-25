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
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? Pono { get; set; }
        public ulong SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int? Status { get; set; }
        public ulong? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public ulong? PaymentTermId { get; set; }
        public ulong? DeliveryTermId { get; set; }
        public string? DeliveryAddress { get; set; }
        public ulong? EmployeeId { get; set; }
        public DateTime? RequestedDate { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? ShippingAmount { get; set; }
        public decimal? OtherAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Remark { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public List<PurchaseOrderDetailDto>? PurchaseOrderDetails { get; set; }
    }

    public class PurchaseOrderRequest
    {
        public string Pono { get; set; } = string.Empty;
        public ulong SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int? Status { get; set; }
        public ulong? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public ulong? PaymentTermId { get; set; }
        public ulong? DeliveryTermId { get; set; }
        public string? DeliveryAddress { get; set; }
        public ulong? EmployeeId { get; set; }
        public DateTime? RequestedDate { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? ShippingAmount { get; set; }
        public decimal? OtherAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Remark { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public List<PurchaseOrderDetailRequest>? PurchaseOrderDetailRequests { get; set; }
    }

    public class PurchaseOrderDetailDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ulong PurchaseOrderId { get; set; }
        public int Stt { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public DateTime? RequiredDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public ulong? WarehouseId { get; set; }
        public string? Note { get; set; }
        public List<PurchaseOrderDeliveryScheduleDto>? PurchaseOrderDeliverySchedules{ get; set; }
    }

    public class PurchaseOrderDetailRequest
    {
        public ulong Id { get; set; }
        public ulong PurchaseOrderId { get; set; }
        public int Stt { get; set; }
        public ulong ProductId { get; set; }
        public ulong UnitId { get; set; }
        public DateTime? RequiredDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public ulong? WarehouseId { get; set; }
        public string? Note { get; set; }
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
