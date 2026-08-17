using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.GoodsIssue.DTOs
{
    public class GoodsIssueDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? IssueNo { get; set; }
        public int? IssueType { get; set; }
        public ulong? WarehouseId { get; set; }
        public string? ReferenceType { get; set; }
        public ulong? ReferenceId { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ulong? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
    }

    public class GoodsIssueDetailDto
    {
        public ulong Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ulong? GoodsIssueId { get; set; }
        public ulong? ProductId { get; set; }
        public ulong? UnitId { get; set; }
        public ulong? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Remark { get; set; }
    }

    public class CreateGoodsIssueRequest
    {
        public string? IssueNo { get; set; } = string.Empty;
        public int? IssueType { get; set; }
        public ulong? WarehouseId { get; set; }
        public string? ReferenceType { get; set; }
        public ulong? ReferenceId { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ulong? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public List<GoodsIssueDetailRequest>? GoodsIssueDetails { get; set; }
    }

    public class UpdateGoodsIssueRequest
    {
        public string? IssueNo { get; set; } = string.Empty;
        public int? IssueType { get; set; }
        public ulong? WarehouseId { get; set; }
        public string? ReferenceType { get; set; }
        public ulong? ReferenceId { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public ulong? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public ulong? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public List<GoodsIssueDetailRequest>? GoodsIssueDetails { get; set; }
    }

    public class GoodsIssueDetailRequest
    {
        public ulong Id { get; set; }
        public ulong? GoodsIssueId { get; set; }
        public ulong? ProductId { get; set; }
        public ulong? UnitId { get; set; }
        public ulong? LocationId { get; set; }
        public string? LotNo { get; set; }
        public string? SerialNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Remark { get; set; }
    }
}
