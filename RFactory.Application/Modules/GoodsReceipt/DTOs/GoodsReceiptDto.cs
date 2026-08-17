using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.GoodsReceipt.DTOs;

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

    /// <summary>
    /// The lines to create with the receipt, in the same transaction.
    /// </summary>
    public List<GoodsReceiptLineRequest>? GoodsReceiptDetails { get; set; }
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

    /// <summary>
    /// The complete line set, replacing what the receipt holds today: lines missing from
    /// the list are deleted. Leave it <c>null</c> — not empty — to edit only the header;
    /// an empty list is read as "this receipt now has no lines".
    /// </summary>
    public List<GoodsReceiptLineRequest>? GoodsReceiptDetails { get; set; }
}

/// <summary>
/// A receipt line as it arrives with its receipt. <c>Id</c> is 0 for a line the operator
/// just added; any other value must already belong to the receipt being saved. There is
/// no GoodsReceiptId here on purpose — the receipt owning the payload supplies it.
/// </summary>
public class GoodsReceiptLineRequest
{
    public ulong Id { get; set; }
    public ulong ProductId { get; set; }
    public ulong UnitId { get; set; }
    public ulong? LocationId { get; set; }
    public string? LotNo { get; set; }
    public string? SerialNo { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Remark { get; set; }
    public DateTime? ExpireDate { get; set; }
}
