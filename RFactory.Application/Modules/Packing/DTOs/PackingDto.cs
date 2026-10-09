using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Packing.DTOs
{
    #region Dto
    public partial class PackingCheckDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string? CheckNo { get; set; }

        public ulong? WarehouseId { get; set; }

        public ulong? PickingPlanId { get; set; }

        /// <summary>
        /// 1: Draft; 2 :Checking; 3: Discrepancy; 4: Completed; 5: Cancelled; 6: Closed
        /// </summary>
        public int Status { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ulong? CheckedBy { get; set; }

        public string? Remark { get; set; }
    }

    public partial class PackingCheckItemDto
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

        public ulong PackingCheckId { get; set; }

        public ulong ProductId { get; set; }

        public ulong? UnitId { get; set; }

        public ulong? LotId { get; set; }

        public string? SerialNo { get; set; }

        public decimal? RequiredQty { get; set; }

        public decimal? PickedQty { get; set; }

        public decimal? PackedQty { get; set; }

        public decimal? RemainingQty { get; set; }

        public decimal? DiscrepancyQty { get; set; }

        /// <summary>
        /// 1: Pending; 2: Checking; 3: Short; 4: Excess; 5: Matched; 6: WrongProduct; 7: Completed
        /// </summary>
        public int Status { get; set; }

        public string? Remark { get; set; }
    }

    public partial class PackingPackageDto
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

        public ulong PackingCheckId { get; set; }

        public string? PackageNo { get; set; }

        /// <summary>
        /// 1: Carton; 2: Pallet; 3: Bag; 4: Crate; 5: Other
        /// </summary>
        public int? PackageType { get; set; }

        public string? Barcode { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Volume { get; set; }

        /// <summary>
        /// 1: Open; 2: Packing; 3: Packed; 4: Closed; 5: Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? PackedAt { get; set; }

        public ulong? PackedBy { get; set; }

        public string? Remark { get; set; }
    }

    public partial class PackingPackageItemDto
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

        public ulong PackingPackageId { get; set; }

        public ulong PackingCheckItemId { get; set; }

        public ulong ProductId { get; set; }

        public ulong? UnitId { get; set; }

        public ulong? LotId { get; set; }

        public string? SerialNo { get; set; }

        public decimal? Quantity { get; set; }

        public string? Barcode { get; set; }
        public DateTime? ScannedAt { get; set; }

        public ulong? ScannedBy { get; set; }

        public string? Remark { get; set; }
    }

    public partial class PackingScanLogDto
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

        public ulong PackingCheckId { get; set; }

        public ulong? PackingCheckItemId { get; set; }

        public ulong? PackingPackageId { get; set; }

        public ulong? ProductId { get; set; }

        public ulong? LotId { get; set; }

        public string? SerialNo { get; set; }

        public string? Barcode { get; set; }

        public decimal? Quantity { get; set; }

        /// <summary>
        /// 1: Scan; 2: Manual; 3: Remove; 4: Adjust
        /// </summary>
        public int ScanType { get; set; }

        /// <summary>
        /// 1: Success; 2: WrongProduct; 3: Excess; 4: Duplicate; 5: InvalidBarcode; 6: WrongLot; 7: WrongSerial; 8: NotInOrder
        /// </summary>
        public int Result { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime ScannedAt { get; set; }

        public ulong? ScannedBy { get; set; }
    }
    #endregion

    #region Request
    public partial class PackingCheckRequest
    {

        public string? CheckNo { get; set; }

        public ulong? WarehouseId { get; set; }

        public ulong? PickingPlanId { get; set; }

        /// <summary>
        /// 1: Draft; 2 :Checking; 3: Discrepancy; 4: Completed; 5: Cancelled; 6: Closed
        /// </summary>
        public int Status { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ulong? CheckedBy { get; set; }

        public string? Remark { get; set; }
        public List<PackingCheckItemRequest>? PackingCheckItems { get; set; }
        public List<PackingPackageRequest>? PackingPackages { get; set; }
    }

    public partial class PackingCheckItemRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public ulong? PackingCheckId { get; set; }

        public ulong ProductId { get; set; }

        public ulong? UnitId { get; set; }

        public ulong? LotId { get; set; }

        public string? SerialNo { get; set; }

        public decimal? RequiredQty { get; set; }

        public decimal? PickedQty { get; set; }

        public decimal? PackedQty { get; set; }

        public decimal? RemainingQty { get; set; }

        public decimal? DiscrepancyQty { get; set; }

        /// <summary>
        /// 1: Pending; 2: Checking; 3: Short; 4: Excess; 5: Matched; 6: WrongProduct; 7: Completed
        /// </summary>
        public int Status { get; set; }

        public string? Remark { get; set; }
        public long? UId { get; set; }
    }

    public partial class PackingPackageRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public ulong? PackingCheckId { get; set; }

        public string? PackageNo { get; set; }

        /// <summary>
        /// 1: Carton; 2: Pallet; 3: Bag; 4: Crate; 5: Other
        /// </summary>
        public int? PackageType { get; set; }

        public string? Barcode { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? Volume { get; set; }

        /// <summary>
        /// 1: Open; 2: Packing; 3: Packed; 4: Closed; 5: Cancelled
        /// </summary>
        public int Status { get; set; }

        public DateTime? PackedAt { get; set; }

        public ulong? PackedBy { get; set; }

        public string? Remark { get; set; }
        public List<PackingPackageItemRequest>? PackingPackageItems { get; set; }
    }

    public partial class PackingPackageItemRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public long? PackingPackageId { get; set; }

        public long? PackingCheckItemId { get; set; }

        public ulong ProductId { get; set; }

        public ulong? UnitId { get; set; }

        public ulong? LotId { get; set; }

        public string? SerialNo { get; set; }

        public decimal? Quantity { get; set; }

        public string? Barcode { get; set; }
        public DateTime? ScannedAt { get; set; }

        public ulong? ScannedBy { get; set; }

        public string? Remark { get; set; }
    }

    public partial class PackingScanLogRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        public ulong PackingCheckId { get; set; }

        public ulong? PackingCheckItemId { get; set; }

        public ulong? PackingPackageId { get; set; }

        public ulong? ProductId { get; set; }

        public ulong? LotId { get; set; }

        public string? SerialNo { get; set; }

        public string? Barcode { get; set; }

        public decimal? Quantity { get; set; }

        /// <summary>
        /// 1: Scan; 2: Manual; 3: Remove; 4: Adjust
        /// </summary>
        public int ScanType { get; set; }

        /// <summary>
        /// 1: Success; 2: WrongProduct; 3: Excess; 4: Duplicate; 5: InvalidBarcode; 6: WrongLot; 7: WrongSerial; 8: NotInOrder
        /// </summary>
        public int Result { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime ScannedAt { get; set; }

        public ulong? ScannedBy { get; set; }
    }
    #endregion
}
