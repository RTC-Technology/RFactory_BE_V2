using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.DeliveryNote.DTOs
{
    #region Dto
    public class DeliveryNoteDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeliveryNo { get; set; }
        public ulong? WarehouseId { get; set; }
        public int Status { get; set; }
        public string? VehicleNo { get; set; }
        public int? VehicleType { get; set; }
        public int? TransportMethod { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public ulong? PickerId { get; set; }
        public string? Remark { get; set; }
    }

    public class DeliveryNoteItemDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public ulong? GoodsIssueId { get; set; }

        public ulong? GoodsIssueDetailId { get; set; }

        public ulong ProductId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public ulong? UnitId { get; set; }

        public string? UnitName { get; set; }
        public decimal? OrderedQty { get; set; }

        public decimal? DeliveredQty { get; set; }

        public string? Remark { get; set; }
    }

    public class DeliveryNoteReceiverDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public string? ReceiverName { get; set; }

        public string? ContactName { get; set; }

        public string? Email1 { get; set; }
        public string? Email2 { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }
        public string? Address1 { get; set; }

        public string? Address2 { get; set; }
    }

    public partial class DeliveryNoteSenderDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public string? SenderName { get; set; }

        public string? Email1 { get; set; }

        public string? Email2 { get; set; }
        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
    }

    public partial class DeliveryNoteSourceDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public ulong GoodsIssueId { get; set; }

        public string? GoodsIssueNo { get; set; }
    }
    #endregion

    #region Request

    public class DeliveryNoteRequest
    {
        public string? DeliveryNo { get; set; }
        public ulong? WarehouseId { get; set; }
        public int Status { get; set; }
        public string? VehicleNo { get; set; }
        public int? VehicleType { get; set; }
        public int? TransportMethod { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public ulong? PickerId { get; set; }
        public string? Remark { get; set; }

        public DeliveryNoteSenderRequest? DeliveryNoteSender { get; set; }
        public DeliveryNoteReceiverRequest? DeliveryNoteReceiver { get; set; }
        public List<DeliveryNoteSourceRequest>? DeliveryNoteSources { get; set; }
        public List<DeliveryNoteItemRequest>? DeliveryNoteItems { get; set; }
    }

    public class DeliveryNoteItemRequest
    {
        public ulong Id { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public ulong? GoodsIssueId { get; set; }

        public ulong? GoodsIssueDetailId { get; set; }

        public ulong ProductId { get; set; }

        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public ulong? UnitId { get; set; }

        public string? UnitName { get; set; }
        public decimal? OrderedQty { get; set; }

        public decimal? DeliveredQty { get; set; }

        public string? Remark { get; set; }
    }

    public class DeliveryNoteReceiverRequest
    {
        public ulong Id { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public string? ReceiverName { get; set; }

        public string? ContactName { get; set; }

        public string? Email1 { get; set; }
        public string? Email2 { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }
        public string? Address1 { get; set; }

        public string? Address2 { get; set; }
    }

    public partial class DeliveryNoteSenderRequest
    {
        public ulong Id { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public string? SenderName { get; set; }

        public string? Email1 { get; set; }

        public string? Email2 { get; set; }
        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
    }

    public partial class DeliveryNoteSourceRequest
    {
        public ulong Id { get; set; }

        public ulong DeliveryNoteId { get; set; }

        public ulong GoodsIssueId { get; set; }

        public string? GoodsIssueNo { get; set; }
    }

    #endregion
}
