using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Production.DTOs
{
    public class ProductionRequestDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Production request number
        /// </summary>
        public string RequestNo { get; set; }

        /// <summary>
        /// Source sales order ID
        /// </summary>
        public ulong? SalesOrderId { get; set; }

        /// <summary>
        /// Source sales order number
        /// </summary>
        public string SalesOrderNo { get; set; }

        /// <summary>
        /// Customer ID
        /// </summary>
        public ulong? CustomerId { get; set; }

        /// <summary>
        /// Production request date
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Required production completion date
        /// </summary>
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// Required production completion date
        /// </summary>
        public DateTime? ReceiptMaterialDate { get; set; }

        /// <summary>
        /// Priority: 1-Urgent, 2-High, 3-Normal, 4-Low
        /// </summary>
        public byte Priority { get; set; }

        /// <summary>
        /// Status: 1-Draft, 2-PendingApproval, 3-Approved, 4-Rejected, 5-Cancelled, 6-Closed
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Remark { get; set; }
    }

    public class ProductionRequestItemDto
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
        /// Production request ID
        /// </summary>
        public ulong? ProductionRequestId { get; set; }

        /// <summary>
        /// Source sales order item ID
        /// </summary>
        public ulong? SalesOrderItemId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Unit of measure ID
        /// </summary>
        public ulong? UnitId { get; set; }

        /// <summary>
        /// Requested production quantity
        /// </summary>
        public decimal RequestedQuantity { get; set; }

        /// <summary>
        /// Quantity already included in production plans
        /// </summary>
        public decimal PlannedQuantity { get; set; }

        /// <summary>
        /// Quantity already produced
        /// </summary>
        public decimal ProducedQuantity { get; set; }

        /// <summary>
        /// Required completion date
        /// </summary>
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// Status: 1-Pending, 2-PartiallyPlanned, 3-Planned, 4-InProgress, 5-Completed, 6-Cancelled
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public uint SortOrder { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Remark { get; set; }
    }


    public class ProductionRequestRequest
    {
        
        /// <summary>
        /// Production request number
        /// </summary>
        public string RequestNo { get; set; }

        /// <summary>
        /// Source sales order ID
        /// </summary>
        public ulong? SalesOrderId { get; set; }

        /// <summary>
        /// Source sales order number
        /// </summary>
        public string SalesOrderNo { get; set; }

        /// <summary>
        /// Customer ID
        /// </summary>
        public ulong? CustomerId { get; set; }

        /// <summary>
        /// Production request date
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Required production completion date
        /// </summary>
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// Required production completion date
        /// </summary>
        public DateTime? ReceiptMaterialDate { get; set; }

        /// <summary>
        /// Priority: 1-Urgent, 2-High, 3-Normal, 4-Low
        /// </summary>
        public byte Priority { get; set; }

        /// <summary>
        /// Status: 1-Draft, 2-PendingApproval, 3-Approved, 4-Rejected, 5-Cancelled, 6-Closed
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Remark { get; set; }
        public List<ProductionRequestItemRequest>? ProductionRequestItems { get; set; }
    }

    public class ProductionRequestItemRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Production request ID
        /// </summary>
        public ulong? ProductionRequestId { get; set; }

        /// <summary>
        /// Source sales order item ID
        /// </summary>
        public ulong? SalesOrderItemId { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public ulong ProductId { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Unit of measure ID
        /// </summary>
        public ulong? UnitId { get; set; }

        /// <summary>
        /// Requested production quantity
        /// </summary>
        public decimal RequestedQuantity { get; set; }

        /// <summary>
        /// Quantity already included in production plans
        /// </summary>
        public decimal PlannedQuantity { get; set; }

        /// <summary>
        /// Quantity already produced
        /// </summary>
        public decimal ProducedQuantity { get; set; }

        /// <summary>
        /// Required completion date
        /// </summary>
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// Status: 1-Pending, 2-PartiallyPlanned, 3-Planned, 4-InProgress, 5-Completed, 6-Cancelled
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public uint SortOrder { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Remark { get; set; }
    }
}
