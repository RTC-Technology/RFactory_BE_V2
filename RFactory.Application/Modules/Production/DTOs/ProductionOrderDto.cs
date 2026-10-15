using System;
using System.Collections.Generic;

namespace RFactory.Application.Modules.Production.DTOs;

public class ProductionOrderDto
{
    public ulong Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// Loại lệnh: 1=MTS, 2=MTO, 3=MRP, 4=Other/Direct
    /// </summary>
    public int OrderType { get; set; } = 4;
    public string OrderTypeName => OrderType switch
    {
        1 => "Kế hoạch tổng thể (MTS)",
        2 => "Đơn đặt hàng (MTO)",
        3 => "Kế hoạch chi tiết (MRP)",
        4 => "Lệnh khác / Trực tiếp",
        _ => "Lệnh khác"
    };

    public string? Description { get; set; }

    public ulong ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }

    public ulong? UnitId { get; set; }
    public string? UnitName { get; set; }

    public ulong? BomId { get; set; }
    public string? BomCode { get; set; }
    public string? BomName { get; set; }
    public string? BomVersion { get; set; }

    public ulong? RoutingId { get; set; }
    public string? RoutingVersion { get; set; }

    public ulong? FactoryId { get; set; }
    public string? FactoryName { get; set; }

    public ulong? LineId { get; set; }
    public string? LineCode { get; set; }
    public string? LineName { get; set; }

    public ulong? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }

    public ulong? CustomerId { get; set; }
    public string? CustomerName { get; set; }

    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    public decimal ProgressPercentage => PlannedQuantity > 0 
        ? Math.Round(Math.Min(100m, (CompletedQuantity / PlannedQuantity) * 100m), 2) 
        : 0m;

    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    public double? DurationHours => PlannedStartDate.HasValue && PlannedEndDate.HasValue && PlannedEndDate > PlannedStartDate
        ? Math.Round((PlannedEndDate.Value - PlannedStartDate.Value).TotalHours, 1)
        : null;

    public int Priority { get; set; }
    public string PriorityName => Priority switch
    {
        1 => "Low",
        2 => "Normal",
        3 => "High",
        4 => "Urgent",
        _ => "Normal"
    };

    public int Status { get; set; }
    public string StatusName => Status switch
    {
        1 => "Draft",
        2 => "Released",
        3 => "InProgress",
        4 => "Completed",
        5 => "Cancelled",
        6 => "Closed",
        7 => "Paused",
        _ => "Unknown"
    };

    public string? BatchNo { get; set; }
    public string? SalesOrderNo { get; set; }
    public string? Remark { get; set; }

    /// <summary>
    /// Tab 1: Chi tiết nguyên vật liệu (BOM Materials)
    /// </summary>
    public List<ProductionOrderMaterialDto> Materials { get; set; } = new();

    /// <summary>
    /// Tab 2: Công đoạn sản xuất (Routing Operations)
    /// </summary>
    public List<ProductionOrderOperationDto> Operations { get; set; } = new();
}

public class ProductionOrderMaterialDto
{
    public ulong Id { get; set; }
    public ulong ProductionOrderId { get; set; }
    public ulong ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public ulong? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal NormQuantity { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public decimal CurrentStock { get; set; }
    public ulong? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public bool IsSubstituted { get; set; }
    public ulong? OriginalProductId { get; set; }
    public string? OriginalProductName { get; set; }
    public string? Remark { get; set; }
}

public class ProductionOrderOperationDto
{
    public ulong Id { get; set; }
    public ulong ProductionOrderId { get; set; }
    public ulong? RoutingOperationId { get; set; }
    public int Sequence { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public ulong? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public ulong? MachineId { get; set; }
    public string? MachineName { get; set; }
    public ulong? AssignedEmployeeId { get; set; }
    public string? AssignedEmployeeName { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int Status { get; set; }
    public string StatusName => Status switch
    {
        1 => "Pending",
        2 => "InProgress",
        3 => "Completed",
        4 => "Skipped",
        _ => "Pending"
    };
    public string? Remark { get; set; }
}

public class ProductionOrderMaterialInputDto
{
    public ulong? Id { get; set; }
    public ulong ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public ulong? UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal NormQuantity { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal? IssuedQuantity { get; set; }
    public decimal? CurrentStock { get; set; }
    public ulong? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public bool IsSubstituted { get; set; }
    public ulong? OriginalProductId { get; set; }
    public string? Remark { get; set; }
}

public class ProductionOrderOperationInputDto
{
    public ulong? Id { get; set; }
    public ulong? RoutingOperationId { get; set; }
    public int Sequence { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public ulong? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public ulong? MachineId { get; set; }
    public string? MachineName { get; set; }
    public ulong? AssignedEmployeeId { get; set; }
    public string? AssignedEmployeeName { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
}

public class ProductionOrderCreateRequest
{
    public string? OrderNo { get; set; }
    public int OrderType { get; set; } = 4; // 1=MTS, 2=MTO, 3=MRP, 4=Other
    public string? Description { get; set; }
    public ulong ProductId { get; set; }
    public ulong? UnitId { get; set; }
    public ulong? BomId { get; set; }
    public ulong? RoutingId { get; set; }
    public ulong? FactoryId { get; set; }
    public ulong? LineId { get; set; }
    public ulong? WorkCenterId { get; set; }
    public ulong? CustomerId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public int Priority { get; set; } = 2; // Normal
    public string? BatchNo { get; set; }
    public string? SalesOrderNo { get; set; }
    public string? Remark { get; set; }

    public List<ProductionOrderMaterialInputDto>? Materials { get; set; }
    public List<ProductionOrderOperationInputDto>? Operations { get; set; }
}

public class ProductionOrderUpdateRequest
{
    public int? OrderType { get; set; }
    public string? Description { get; set; }
    public ulong ProductId { get; set; }
    public ulong? UnitId { get; set; }
    public ulong? BomId { get; set; }
    public ulong? RoutingId { get; set; }
    public ulong? FactoryId { get; set; }
    public ulong? LineId { get; set; }
    public ulong? WorkCenterId { get; set; }
    public ulong? CustomerId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal? CompletedQuantity { get; set; }
    public decimal? ScrapQuantity { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int Priority { get; set; } = 2;
    public string? BatchNo { get; set; }
    public string? SalesOrderNo { get; set; }
    public string? Remark { get; set; }

    public List<ProductionOrderMaterialInputDto>? Materials { get; set; }
    public List<ProductionOrderOperationInputDto>? Operations { get; set; }
}

public class ProductionOrderStatusRequest
{
    public int Status { get; set; }
    public string? Remark { get; set; }
}

public class ProductionOrderProgressRequest
{
    public decimal CompletedQuantityDelta { get; set; }
    public decimal ScrapQuantityDelta { get; set; }
    public string? BatchNo { get; set; }
    public string? Remark { get; set; }
}

public class ProductionOrderFilterDto
{
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public int? OrderType { get; set; }
    public ulong? ProductId { get; set; }
    public ulong? LineId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchKeyword { get; set; }
}
