using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Production.DTOs
{
    public class SalesOrderDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Số đơn đặt hàng
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public ulong CustomerId { get; set; }

        /// <summary>
        /// Đơn hàng cha
        /// </summary>
        public ulong? ParentId { get; set; }

        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Ngày giao hàng dự kiến
        /// </summary>
        public DateTime? ExpectedDeliveryDate { get; set; }

        /// <summary>
        /// Loại đơn hàng: 1=Standard, 2=Additional, 3=Adjustment, 4=Scheduled Delivery
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// Trạng thái đơn hàng: 1=Draft, 2=Confirmed, 3=In Progress, 4=Partially Completed, 5=Completed, 6=Cancelled, 7=Closed
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Tình trạng thực hiện đơn hàng: 1=Not Started, 2=In Progress, 3=Partially Completed, 4=Completed, 5=Cancelled
        /// </summary>
        public int ExecutionStatus { get; set; }

        /// <summary>
        /// Tình trạng ghi nhận doanh thu: 1=Not Recognized, 2=Partially Recognized, 3=Fully Recognized, 4=Cancelled
        /// </summary>
        public int RevenueStatus { get; set; }

        /// <summary>
        /// Tiền tệ
        /// </summary>
        public ulong? CurrencyId { get; set; }

        /// <summary>
        /// Tỷ giá
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Tổng giá trị đơn hàng
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Tổng tiền chiết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// Số tiền còn phải thanh toán
        /// </summary>
        public decimal OutstandingAmount { get; set; }

        /// <summary>
        /// Số ngày được nợ
        /// </summary>
        public int? PaymentTermDays { get; set; }

        /// <summary>
        /// Hạn thanh toán
        /// </summary>
        public DateTime? PaymentDueDate { get; set; }

        /// <summary>
        /// Cơ hội bán hàng
        /// </summary>
        public ulong? OpportunityId { get; set; }

        /// <summary>
        /// Chiến dịch
        /// </summary>
        public ulong? CampaignId { get; set; }

        /// <summary>
        /// Báo giá
        /// </summary>
        public ulong? QuotationId { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        public string? Description { get; set; }
    }

    public class SalesOrderItemDto
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
        /// Đơn đặt hàng
        /// </summary>
        public ulong SalesOrderId { get; set; }

        /// <summary>
        /// Sản phẩm
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Tên sản phẩm tại thời điểm đặt hàng
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public ulong? UnitId { get; set; }

        /// <summary>
        /// Kho xuất
        /// </summary>
        public ulong? WarehouseId { get; set; }

        /// <summary>
        /// Số lượng đặt
        /// </summary>
        public decimal OrderedQuantity { get; set; }

        /// <summary>
        /// Số lượng đã giao
        /// </summary>
        public decimal DeliveredQuantity { get; set; }

        /// <summary>
        /// Số lượng đã hủy
        /// </summary>
        public decimal CancelledQuantity { get; set; }

        /// <summary>
        /// Đơn giá
        /// </summary>
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// Tỷ lệ chiết khấu
        /// </summary>
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// Tiền chiết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Thuế suất
        /// </summary>
        public decimal TaxPercent { get; set; }

        /// <summary>
        /// Tiền thuế
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Ngày giao dự kiến
        /// </summary>
        public DateTime? ExpectedDeliveryDate { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        public string? Description { get; set; }
    }

    public class SalesOrderPaymentScheduleDto
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
        /// Đơn đặt hàng
        /// </summary>
        public ulong SalesOrderId { get; set; }

        /// <summary>
        /// Thứ tự đợt thanh toán
        /// </summary>
        public int ScheduleNo { get; set; }

        /// <summary>
        /// Tên đợt thanh toán
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// Tỷ lệ thanh toán
        /// </summary>
        public decimal PaymentPercent { get; set; }

        /// <summary>
        /// Giá trị thanh toán
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Hạn thanh toán
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// Trạng thái thanh toán:1 = Pending; 2 = Due; 3 = PartiallyPaid; 4 = Paid; 5 = Overdue; 6 = Cancelled
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        public string? Description { get; set; }
    }


    public class SalesOrderRequest
    {
        public string OrderNo { get; set; }
        public ulong CustomerId { get; set; }
        public ulong? ParentId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int OrderType { get; set; }
        public int Status { get; set; }
        public int ExecutionStatus { get; set; }
        public int RevenueStatus { get; set; }
        public ulong? CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public int? PaymentTermDays { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public ulong? OpportunityId { get; set; }
        public ulong? CampaignId { get; set; }
        public ulong? QuotationId { get; set; }
        public string? Description { get; set; }
        public List<SalesOrderItemRequest>? SalesOrderItems { get; set; }
        public List<SalesOrderPaymentScheduleRequest>? SalesOrderPaymentSchedules { get; set; }
    }

    public class SalesOrderItemRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public ulong? SalesOrderId { get; set; }

        /// <summary>
        /// Sản phẩm
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Tên sản phẩm tại thời điểm đặt hàng
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public ulong? UnitId { get; set; }

        /// <summary>
        /// Kho xuất
        /// </summary>
        public ulong? WarehouseId { get; set; }

        /// <summary>
        /// Số lượng đặt
        /// </summary>
        public decimal OrderedQuantity { get; set; }

        /// <summary>
        /// Số lượng đã giao
        /// </summary>
        public decimal DeliveredQuantity { get; set; }

        /// <summary>
        /// Số lượng đã hủy
        /// </summary>
        public decimal CancelledQuantity { get; set; }

        /// <summary>
        /// Đơn giá
        /// </summary>
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// Tỷ lệ chiết khấu
        /// </summary>
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// Tiền chiết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Thuế suất
        /// </summary>
        public decimal TaxPercent { get; set; }

        /// <summary>
        /// Tiền thuế
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Ngày giao dự kiến
        /// </summary>
        public DateTime? ExpectedDeliveryDate { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        public string? Description { get; set; }
    }

    public class SalesOrderPaymentScheduleRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public ulong? SalesOrderId { get; set; }

        /// <summary>
        /// Thứ tự đợt thanh toán
        /// </summary>
        public int ScheduleNo { get; set; }

        /// <summary>
        /// Tên đợt thanh toán
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// Tỷ lệ thanh toán
        /// </summary>
        public decimal PaymentPercent { get; set; }

        /// <summary>
        /// Giá trị thanh toán
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Hạn thanh toán
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// Trạng thái thanh toán:1 = Pending; 2 = Due; 3 = PartiallyPaid; 4 = Paid; 5 = Overdue; 6 = Cancelled
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        public string? Description { get; set; }
    }
}
