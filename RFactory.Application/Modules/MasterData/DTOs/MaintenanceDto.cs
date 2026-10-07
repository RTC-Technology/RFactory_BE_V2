using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.DTOs
{
    public class MaintenanceTypeDto
    {
        public ulong Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Mã loại bảo trì
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Tên loại bảo trì
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Nhóm loại bảo trì: 1 Planned, 2 Unplanned, 3 Condition, 4 Special
        /// </summary>
        public int? TypeCategory { get; set; }

        /// <summary>
        /// Mức độ ưu tiên mặc định: 1:low, 2:normal; 3:high; 4:critical
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class MaintenanceChecklistDto
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
        /// Mã checklist
        /// </summary>
        public string ChecklistCode { get; set; }

        /// <summary>
        /// Tên checklist
        /// </summary>
        public string ChecklistName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Phiên bản checklist
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Trạng thái: 1 Draft, 2 Active, 3 Inactive
        /// </summary>
        public int Status { get; set; }
    }

    public class MaintenanceChecklistItemDto
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
        /// Checklist
        /// </summary>
        public ulong? MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Thứ tự thực hiện
        /// </summary>
        public int SequenceNo { get; set; }

        /// <summary>
        /// Mã hạng mục
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Tên hạng mục kiểm tra
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Loại kiểm tra: 1 YesNo, 2 Numeric, 3 Text, 4 Selection, 5 PassFail, 6 Inspection
        /// </summary>
        public int CheckType { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public ulong? UnitId { get; set; }

        /// <summary>
        /// Giá trị tiêu chuẩn
        /// </summary>
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Giá trị tối thiểu
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Giá trị tối đa
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Kết quả mong đợi
        /// </summary>
        public string? ExpectedResult { get; set; }

        /// <summary>
        /// Bắt buộc thực hiện
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class MaintenancePlanDto
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
        /// Mã kế hoạch bảo trì
        /// </summary>
        public string PlanCode { get; set; }

        /// <summary>
        /// Tên kế hoạch bảo trì
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// Loại bảo trì
        /// </summary>
        public ulong MaintenanceTypeId { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ulong MachineId { get; set; }

        /// <summary>
        /// Work Center
        /// </summary>
        public ulong? WorkCenterId { get; set; }

        /// <summary>
        /// Checklist sử dụng
        /// </summary>
        public ulong? MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Chu kỳ bảo trì
        /// </summary>
        public decimal Frequency { get; set; }

        /// <summary>
        /// Đơn vị chu kỳ: 1 Day, 2 Week, 3 Month, 4 Year, 5 Hour, 6 ProductionQuantity
        /// </summary>
        public int FrequencyUnit { get; set; }

        /// <summary>
        /// Ngày bắt đầu áp dụng
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc áp dụng
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Ngày bảo trì gần nhất
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// Ngày bảo trì tiếp theo
        /// </summary>
        public DateTime? NextMaintenanceDate { get; set; }

        /// <summary>
        /// Mức độ ưu tiên: 1:low, 2:normal; 3:high; 4:critical
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Trạng thái: 1 Draft, 2 Active, 3 Inactive, 4 Expired
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Bộ phận phụ trách
        /// </summary>
        public ulong? ResponsibleDepartmentId { get; set; }

        /// <summary>
        /// Nhân viên phụ trách
        /// </summary>
        public ulong? ResponsibleEmployeeId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Remark { get; set; }
    }

    #region Request
    public class MaintenanceTypeRequest
    {

        /// <summary>
        /// Mã loại bảo trì
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Tên loại bảo trì
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Tên viết tắt
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Nhóm loại bảo trì: 1 Planned, 2 Unplanned, 3 Condition, 4 Special
        /// </summary>
        public int? TypeCategory { get; set; }

        /// <summary>
        /// Mức độ ưu tiên mặc định: 1:low, 2:normal; 3:high; 4:critical
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class MaintenanceChecklistRequest
    {

        /// <summary>
        /// Mã checklist
        /// </summary>
        public string ChecklistCode { get; set; }

        /// <summary>
        /// Tên checklist
        /// </summary>
        public string ChecklistName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Phiên bản checklist
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Trạng thái: 1 Draft, 2 Active, 3 Inactive
        /// </summary>
        public int Status { get; set; }
        public List<MaintenanceChecklistItemRequest>? MaintenanceChecklistItems { get; set; }
    }

    public class MaintenanceChecklistItemRequest
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Checklist
        /// </summary>
        public ulong? MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Thứ tự thực hiện
        /// </summary>
        public int SequenceNo { get; set; }

        /// <summary>
        /// Mã hạng mục
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Tên hạng mục kiểm tra
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Loại kiểm tra: 1 YesNo, 2 Numeric, 3 Text, 4 Selection, 5 PassFail, 6 Inspection
        /// </summary>
        public int CheckType { get; set; }

        /// <summary>
        /// Đơn vị đo
        /// </summary>
        public ulong? UnitId { get; set; }

        /// <summary>
        /// Giá trị tiêu chuẩn
        /// </summary>
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Giá trị tối thiểu
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Giá trị tối đa
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Kết quả mong đợi
        /// </summary>
        public string? ExpectedResult { get; set; }

        /// <summary>
        /// Bắt buộc thực hiện
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool? IsActive { get; set; }
    }

    public class MaintenancePlanRequest
    {

        /// <summary>
        /// Mã kế hoạch bảo trì
        /// </summary>
        public string PlanCode { get; set; }

        /// <summary>
        /// Tên kế hoạch bảo trì
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// Loại bảo trì
        /// </summary>
        public ulong MaintenanceTypeId { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ulong MachineId { get; set; }

        /// <summary>
        /// Work Center
        /// </summary>
        public ulong? WorkCenterId { get; set; }

        /// <summary>
        /// Checklist sử dụng
        /// </summary>
        public ulong? MaintenanceChecklistId { get; set; }

        /// <summary>
        /// Chu kỳ bảo trì
        /// </summary>
        public decimal Frequency { get; set; }

        /// <summary>
        /// Đơn vị chu kỳ: 1 Day, 2 Week, 3 Month, 4 Year, 5 Hour, 6 ProductionQuantity
        /// </summary>
        public int FrequencyUnit { get; set; }

        /// <summary>
        /// Ngày bắt đầu áp dụng
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc áp dụng
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Ngày bảo trì gần nhất
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// Ngày bảo trì tiếp theo
        /// </summary>
        public DateTime? NextMaintenanceDate { get; set; }

        /// <summary>
        /// Mức độ ưu tiên: 1:low, 2:normal; 3:high; 4:critical
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Trạng thái: 1 Draft, 2 Active, 3 Inactive, 4 Expired
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Bộ phận phụ trách
        /// </summary>
        public ulong? ResponsibleDepartmentId { get; set; }

        /// <summary>
        /// Nhân viên phụ trách
        /// </summary>
        public ulong? ResponsibleEmployeeId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Remark { get; set; }
    }
    #endregion
}
