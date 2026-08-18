using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Inventory.DTOs
{
    public class InventoryDto
    {
        public ulong Id { get; set; }
        public long? ProductId { get; set; }
        public long? WarehouseId { get; set; }
        public long? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public long? UnitId { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }

    public class CreateInventoryRequest
    {
        public ulong Id { get; set; }
        public long? ProductId { get; set; }
        public long? WarehouseId { get; set; }
        public long? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public long? UnitId { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }

    public class UpdateInventoryRequest
    {
        public ulong Id { get; set; }
        public long? ProductId { get; set; }
        public long? WarehouseId { get; set; }
        public long? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public long? UnitId { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }

    public class InventoryTransactionDto
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }
        public string? TransactionNo { get; set; }
        /// <summary>
        /// 1: RECEIPT; 2: ISSUE; 3: TRANSFER; 4: ADJUST; 5: PRODUCTION_IN; 6: PRODUCTION_OUT; 7: SCRAP
        /// </summary>
        public int? TransactionType { get; set; }
        public long? ProductId { get; set; }
        public long? WarehouseId { get; set; }
        public long? WarehouseLocationId { get; set; }
        public decimal? Quantity { get; set; }
        public long? UnitId { get; set; }
        /// <summary>
        /// 1: GR; 2: GI; 3: TRANSFER_IN; 4: TRANSFER_OUT; 5: MATERIAL_ISSUE; 6: MATERIAL_RETURN; 7: PRODUCTION_RECEIPT; 8: PRODUCTION_CONSUME; 9: ADJUSTMENT; 10: SCRAP\n
        /// </summary>
        public int ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    public class CreateInventoryTransactionRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }
        public string? TransactionNo { get; set; }
        /// <summary>
        /// 1: RECEIPT; 2: ISSUE; 3: TRANSFER; 4: ADJUST; 5: PRODUCTION_IN; 6: PRODUCTION_OUT; 7: SCRAP
        /// </summary>
        public int? TransactionType { get; set; }
        public long? ProductId { get; set; }
        public long? WarehouseId { get; set; }
        public long? WarehouseLocationId { get; set; }
        public decimal? Quantity { get; set; }
        public long? UnitId { get; set; }
        /// <summary>
        /// 1: GR; 2: GI; 3: TRANSFER_IN; 4: TRANSFER_OUT; 5: MATERIAL_ISSUE; 6: MATERIAL_RETURN; 7: PRODUCTION_RECEIPT; 8: PRODUCTION_CONSUME; 9: ADJUSTMENT; 10: SCRAP\n
        /// </summary>
        public int ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    public class UpdateInventoryTransactionRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }
        public string? TransactionNo { get; set; }
        /// <summary>
        /// 1: RECEIPT; 2: ISSUE; 3: TRANSFER; 4: ADJUST; 5: PRODUCTION_IN; 6: PRODUCTION_OUT; 7: SCRAP
        /// </summary>
        public int? TransactionType { get; set; }
        public long? ProductId { get; set; }
        public long? WarehouseId { get; set; }
        public long? WarehouseLocationId { get; set; }
        public decimal? Quantity { get; set; }
        public long? UnitId { get; set; }
        /// <summary>
        /// 1: GR; 2: GI; 3: TRANSFER_IN; 4: TRANSFER_OUT; 5: MATERIAL_ISSUE; 6: MATERIAL_RETURN; 7: PRODUCTION_RECEIPT; 8: PRODUCTION_CONSUME; 9: ADJUSTMENT; 10: SCRAP\n
        /// </summary>
        public int ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    /// <summary>
    /// Inventory transaction type.
    /// </summary>
    public enum InventoryTransactionType
    {
        /// <summary>
        /// 1: RECEIPT - Nhập kho
        /// </summary>
        Receipt = 1,

        /// <summary>
        /// 2: ISSUE - Xuất kho
        /// </summary>
        Issue = 2,

        /// <summary>
        /// 3: TRANSFER - Chuyển kho
        /// </summary>
        Transfer = 3,

        /// <summary>
        /// 4: ADJUST - Điều chỉnh tồn kho
        /// </summary>
        Adjust = 4,

        /// <summary>
        /// 5: PRODUCTION_IN - Nhập kho sản xuất
        /// </summary>
        ProductionIn = 5,

        /// <summary>
        /// 6: PRODUCTION_OUT - Xuất kho sản xuất
        /// </summary>
        ProductionOut = 6,

        /// <summary>
        /// 7: SCRAP - Hủy
        /// </summary>
        Scrap = 7
    }

    /// <summary>
    /// Inventory transaction reference type.
    /// </summary>
    public enum InventoryReferenceType
    {
        /// <summary>
        /// 1: GR - Goods Receipt
        /// </summary>
        GoodsReceipt = 1,

        /// <summary>
        /// 2: GI - Goods Issue
        /// </summary>
        GoodsIssue = 2,

        /// <summary>
        /// 3: TRANSFER_IN - Transfer In
        /// </summary>
        TransferIn = 3,

        /// <summary>
        /// 4: TRANSFER_OUT - Transfer Out
        /// </summary>
        TransferOut = 4,

        /// <summary>
        /// 5: MATERIAL_ISSUE - Material Issue
        /// </summary>
        MaterialIssue = 5,

        /// <summary>
        /// 6: MATERIAL_RETURN - Material Return
        /// </summary>
        MaterialReturn = 6,

        /// <summary>
        /// 7: PRODUCTION_RECEIPT - Production Receipt
        /// </summary>
        ProductionReceipt = 7,

        /// <summary>
        /// 8: PRODUCTION_CONSUME - Production Consume
        /// </summary>
        ProductionConsume = 8,

        /// <summary>
        /// 9: ADJUSTMENT - Adjustment
        /// </summary>
        Adjustment = 9,

        /// <summary>
        /// 10: SCRAP - Scrap
        /// </summary>
        Scrap = 10
    }
}
