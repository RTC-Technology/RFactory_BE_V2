using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.PickingPlan.DTOs
{
    public class PickingPlanDto
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

        public string? PlanNo { get; set; }

        public int? WarehouseId { get; set; }

        /// <summary>
        /// 1 :(Draft - Nháp), 2: (Approved - Đã duyệt), 3 :(InProgress - Đang lấy hàng), 4: (PartiallyPicked - Lấy một phần), 5: (FullyPicked - Lấy đủ), 6: (Cancelled - Đã hủy), 7: (Closed - Đã đóng)
        /// </summary>
        public int Status { get; set; }

        public string? Remark { get; set; }
    }

    public class PickingPlanItemDto
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

        public int? PickingPlanId { get; set; }

        public int? ProductId { get; set; }

        public int? UnitId { get; set; }

        public decimal? RequiredQty { get; set; }

        public decimal? AllocatedQty { get; set; }

        public decimal? PickedQty { get; set; }

        public decimal? RemainingQty { get; set; }

        /// <summary>
        /// 1:Pending, 2:Picking, 3:PartiallyPicked, 4:FullyPicked, 5:Cancelled
        /// </summary>
        public int? Status { get; set; }

        public string? Remark { get; set; }
    }

    public class PickingPlanItemSourceDto
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

        public int? PickingPlanItemId { get; set; }

        public int? SourceId { get; set; }

        public int? SourceDetailId { get; set; }

        public decimal? RequiredQty { get; set; }

        public decimal? PickedQty { get; set; }
    }

    public class PickingPlanSourceDto
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

        public int? PickingPlanId { get; set; }

        /// <summary>
        /// 1:Goods Issue; 2: Tranfer Request
        /// </summary>
        public int? SourceType { get; set; }

        public int? SourceId { get; set; }

        public string? SourceNo { get; set; }
    }

    public class PickingTicketDto
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

        public string? TicketNo { get; set; }

        public int? PickingPlanId { get; set; }

        public int? WarehouseId { get; set; }

        /// <summary>
        /// 1:Draft ( Nháp), 2 :Released ( Đã phát hành), 3 :InProgress - (Đang lấy hàng), 4 :PartiallyPicked ( Lấy một phần), 5 :Completed ( Hoàn thành), 6 :Cancelled ( Đã hủy)
        /// </summary>
        public int Status { get; set; }

        public long? AssignedTo { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Remark { get; set; }
    }

    public class PickingTicketItemDto
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
        public int? PickingTicketId { get; set; }
        public int? PickingPlanItemId { get; set; }
        public int? ProductId { get; set; }
        public int? LocationId { get; set; }
        public int? LotId { get; set; }
        public string? SerialNo { get; set; }
        public decimal? RequestedQty { get; set; }
        public decimal? PickedQty { get; set; }

        /// <summary>
        /// 1:Pending (Chờ lấy), 2:Picking (Đang lấy), 3:PartiallyPicked (Lấy một phần), 4:Picked (Đã lấy đủ), 5:Skipped (Không lấy), 6:Cancelled (Đã hủy)
        /// </summary>
        public int? Status { get; set; }
        public string? Remark { get; set; }
    }


    public class PickingPlanRequest
    {
        public string PlanNo { get; set; } = string.Empty;
        public ulong? WarehouseId { get; set; }
        public int Status { get; set; }
        public string? Remark { get; set; }
        public List<PickingPlanSourceRequest>? PickingPlanSources { get; set; }
        public List<PickingPlanItemRequest>? PickingPlanItems { get; set; }
        public List<PickingTicketRequest>? PickingTickets { get; set; }
    }

    public class PickingPlanSourceRequest
    {
        public ulong Id { get; set; }
        public ulong? PickingPlanId { get; set; }
        public int? SourceType { get; set; }
        public ulong? SourceId { get; set; }
        public string? SourceNo { get; set; }
    }

    public class PickingPlanItemSourceRequest
    {
        public ulong Id { get; set; }
        public ulong? PickingPlanItemId { get; set; }
        public ulong? SourceId { get; set; }
        public ulong? SourceDetailId { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? PickedQty { get; set; }
    }

    public class PickingPlanItemRequest
    {
        public ulong Id { get; set; }
        public ulong? PickingPlanId { get; set; }
        public ulong? ProductId { get; set; }
        public ulong? UnitId { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? AllocatedQty { get; set; }
        public decimal? PickedQty { get; set; }
        public decimal? RemainingQty { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public List<PickingPlanItemSourceRequest>? PickingPlanItemSources { get; set; }
        public long? UId { get; set; }
    }

    public class PickingTicketRequest
    {
        public ulong Id { get; set; }
        public string TicketNo { get; set; } = string.Empty;
        public ulong? PickingPlanId { get; set; }
        public ulong? WarehouseId { get; set; }
        public int Status { get; set; }
        public ulong? AssignedTo { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Remark { get; set; }
        public List<PickingTicketItemRequest> PickingTicketItems { get; set; } = new();
    }

    public class PickingTicketItemRequest
    {
        public ulong Id { get; set; }
        public ulong? PickingTicketId { get; set; }
        public long? PickingPlanItemId { get; set; }
        public ulong? ProductId { get; set; }
        public ulong? LocationId { get; set; }
        public ulong? LotId { get; set; }
        public string? SerialNo { get; set; }
        public decimal? RequestedQty { get; set; }
        public decimal? PickedQty { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
    }

}
