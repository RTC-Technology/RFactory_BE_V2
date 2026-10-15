#nullable disable
using System;

namespace RFactory.Infrastructure.Entities;

/// <summary>
/// Công đoạn sản xuất theo lệnh sản xuất (Production Order Operation)
/// </summary>
public partial class ProductionOrderOperation
{
    public ulong Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public ulong ProductionOrderId { get; set; }

    public ulong? RoutingOperationId { get; set; }

    /// <summary>
    /// Thứ tự công đoạn
    /// </summary>
    public int Sequence { get; set; }

    public string OperationCode { get; set; }

    public string OperationName { get; set; }

    public ulong? WorkCenterId { get; set; }

    public string WorkCenterName { get; set; }

    public ulong? MachineId { get; set; }

    public string MachineName { get; set; }

    public ulong? AssignedEmployeeId { get; set; }

    public string AssignedEmployeeName { get; set; }

    public DateTime? PlannedStartDate { get; set; }

    public DateTime? PlannedEndDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    /// <summary>
    /// Trạng thái: 1=Chưa thực hiện, 2=Đang thực hiện, 3=Hoàn thành, 4=Bỏ qua
    /// </summary>
    public int Status { get; set; }

    public string Remark { get; set; }
}
