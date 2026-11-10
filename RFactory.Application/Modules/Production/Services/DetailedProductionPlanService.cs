using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;
using RFactory.Shared.Exceptions;

namespace RFactory.Application.Modules.Production.Services;

public class DetailedProductionPlanService : IDetailedProductionPlanService
{
    private readonly RFactoryContext _context;
    private readonly IMapper _mapper;

    public DetailedProductionPlanService(RFactoryContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<DetailedProductionPlanDto>> GetAllAsync(DetailedProductionPlanFilterDto? filter = null)
    {
        var query = _context.DetailedProductionPlans
            .Include(p => p.Items)
            .Include(p => p.Schedules)
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(p =>
                    EF.Functions.Like(p.PlanCode, $"%{kw}%") ||
                    EF.Functions.Like(p.PlanName, $"%{kw}%") ||
                    (p.MasterPlanCode != null && EF.Functions.Like(p.MasterPlanCode, $"%{kw}%")) ||
                    (p.WorkshopName != null && EF.Functions.Like(p.WorkshopName, $"%{kw}%")));
            }

            if (filter.WorkshopId.HasValue && filter.WorkshopId.Value > 0)
            {
                query = query.Where(p => p.WorkshopId == filter.WorkshopId.Value);
            }

            if (filter.Status.HasValue && filter.Status.Value > 0)
            {
                query = query.Where(p => p.Status == filter.Status.Value);
            }

            if (filter.FromDate.HasValue)
            {
                var from = filter.FromDate.Value.Date;
                query = query.Where(p => p.ToDate >= from);
            }

            if (filter.ToDate.HasValue)
            {
                var to = filter.ToDate.Value.Date;
                query = query.Where(p => p.FromDate <= to);
            }
        }

        var plans = await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
        return _mapper.Map<List<DetailedProductionPlanDto>>(plans);
    }

    public async Task<DetailedProductionPlanDto?> GetByIdAsync(ulong id)
    {
        var plan = await _context.DetailedProductionPlans
            .Include(p => p.Items.OrderBy(i => i.Sequence))
            .Include(p => p.Schedules.OrderBy(s => s.ScheduleDate).ThenBy(s => s.ShiftName))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null) return null;

        return _mapper.Map<DetailedProductionPlanDto>(plan);
    }

    public async Task<DetailedProductionPlanDto> CreateAsync(DetailedProductionPlanCreateRequest request, string currentUser)
    {
        if (request.FromDate.Date > request.ToDate.Date)
        {
            throw new BusinessException("Từ ngày không thể lớn hơn Đến ngày", 400);
        }

        var planCode = string.IsNullOrWhiteSpace(request.PlanCode)
            ? await GeneratePlanCodeAsync()
            : request.PlanCode.Trim();

        var codeExists = await _context.DetailedProductionPlans.AnyAsync(x => x.PlanCode == planCode && !x.IsDeleted);
        if (codeExists)
        {
            throw new BusinessException($"Kế hoạch chi tiết với mã '{planCode}' đã tồn tại", 409);
        }

        var plan = new DetailedProductionPlan
        {
            PlanCode = planCode,
            PlanName = string.IsNullOrWhiteSpace(request.PlanName) ? $"Kế hoạch chi tiết {planCode}" : request.PlanName.Trim(),
            MasterPlanId = request.MasterPlanId,
            MasterPlanCode = request.MasterPlanCode,
            WorkshopId = request.WorkshopId,
            WorkshopName = request.WorkshopName,
            LineId = request.LineId,
            LineName = request.LineName,
            DepartmentId = request.DepartmentId,
            DepartmentName = request.DepartmentName,
            PeriodType = string.IsNullOrWhiteSpace(request.PeriodType) ? "DAY" : request.PeriodType.Trim().ToUpperInvariant(),
            FromDate = request.FromDate.Date,
            ToDate = request.ToDate.Date,
            AllocationMethod = string.IsNullOrWhiteSpace(request.AllocationMethod) ? "FORWARD" : request.AllocationMethod.Trim().ToUpperInvariant(),
            Status = 1, // Draft
            TotalRequiredQuantity = request.Items.Sum(i => i.RequiredQuantity),
            TotalPlannedQuantity = request.Schedules.Sum(s => s.PlannedQuantity),
            ProductionOrderCount = 0,
            Remark = request.Remark,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.DetailedProductionPlans.Add(plan);
        await _context.SaveChangesAsync();

        // Add Items
        var planItems = new List<DetailedProductionPlanItem>();
        int seq = 1;
        foreach (var itemReq in request.Items)
        {
            var item = _mapper.Map<DetailedProductionPlanItem>(itemReq);
            item.PlanId = plan.Id;
            item.Sequence = seq++;
            item.PlannedQuantity = request.Schedules
                .Where(s => s.ProductId == itemReq.ProductId)
                .Sum(s => s.PlannedQuantity);
            item.CreatedBy = currentUser;
            item.CreatedDate = DateTime.UtcNow;
            item.IsDeleted = false;
            _context.DetailedProductionPlanItems.Add(item);
            planItems.Add(item);
        }
        await _context.SaveChangesAsync();

        // Add Schedules
        if (request.Schedules.Count > 0)
        {
            var itemMap = planItems.ToDictionary(i => i.ProductId, i => i.Id);
            foreach (var schReq in request.Schedules)
            {
                var sch = _mapper.Map<DetailedProductionPlanSchedule>(schReq);
                sch.PlanId = plan.Id;
                if (itemMap.TryGetValue(sch.ProductId, out var planItemId))
                {
                    sch.PlanItemId = planItemId;
                }
                sch.ScheduleDate = schReq.ScheduleDate.Date;
                sch.CreatedBy = currentUser;
                sch.CreatedDate = DateTime.UtcNow;
                sch.IsDeleted = false;
                _context.DetailedProductionPlanSchedules.Add(sch);
            }
            await _context.SaveChangesAsync();
        }

        // Update Master Plan Allocated Quantities if applicable
        if (plan.MasterPlanId.HasValue)
        {
            await SyncMasterPlanAllocatedQuantitiesAsync(plan.MasterPlanId.Value, currentUser);
        }

        var result = await GetByIdAsync(plan.Id);
        return result!;
    }

    public async Task<DetailedProductionPlanDto> UpdateAsync(ulong id, DetailedProductionPlanUpdateRequest request, string currentUser)
    {
        var plan = await _context.DetailedProductionPlans
            .Include(p => p.Items)
            .Include(p => p.Schedules)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null)
            throw new NotFoundException($"Không tìm thấy Kế hoạch chi tiết #{id}");

        if (plan.Status == 4 || plan.Status == 5)
            throw new BusinessException("Không thể chỉnh sửa kế hoạch đã hoàn thành hoặc đã hủy", 400);

        if (request.FromDate.Date > request.ToDate.Date)
            throw new BusinessException("Từ ngày không thể lớn hơn Đến ngày", 400);

        plan.PlanName = request.PlanName.Trim();
        plan.MasterPlanId = request.MasterPlanId;
        plan.MasterPlanCode = request.MasterPlanCode;
        plan.WorkshopId = request.WorkshopId;
        plan.WorkshopName = request.WorkshopName;
        plan.LineId = request.LineId;
        plan.LineName = request.LineName;
        plan.DepartmentId = request.DepartmentId;
        plan.DepartmentName = request.DepartmentName;
        plan.PeriodType = string.IsNullOrWhiteSpace(request.PeriodType) ? "DAY" : request.PeriodType.Trim().ToUpperInvariant();
        plan.FromDate = request.FromDate.Date;
        plan.ToDate = request.ToDate.Date;
        plan.AllocationMethod = string.IsNullOrWhiteSpace(request.AllocationMethod) ? "FORWARD" : request.AllocationMethod.Trim().ToUpperInvariant();
        plan.TotalRequiredQuantity = request.Items.Sum(i => i.RequiredQuantity);
        plan.TotalPlannedQuantity = request.Schedules.Sum(s => s.PlannedQuantity);
        plan.Remark = request.Remark;
        plan.UpdatedBy = currentUser;
        plan.UpdatedDate = DateTime.UtcNow;

        // Replace Items
        _context.DetailedProductionPlanItems.RemoveRange(plan.Items);
        var planItems = new List<DetailedProductionPlanItem>();
        int seq = 1;
        foreach (var itemReq in request.Items)
        {
            var item = _mapper.Map<DetailedProductionPlanItem>(itemReq);
            item.PlanId = plan.Id;
            item.Sequence = seq++;
            item.PlannedQuantity = request.Schedules
                .Where(s => s.ProductId == itemReq.ProductId)
                .Sum(s => s.PlannedQuantity);
            item.CreatedBy = currentUser;
            item.CreatedDate = DateTime.UtcNow;
            item.IsDeleted = false;
            _context.DetailedProductionPlanItems.Add(item);
            planItems.Add(item);
        }
        await _context.SaveChangesAsync();

        // Preserve existing production orders linked to schedules if any
        var existingOrdersBySchedule = plan.Schedules
            .Where(s => s.ProductionOrderId.HasValue)
            .ToDictionary(s => $"{s.ProductId}_{s.ScheduleDate:yyyyMMdd}_{s.ShiftName}", s => new { s.ProductionOrderId, s.ProductionOrderNo });

        // Replace Schedules
        _context.DetailedProductionPlanSchedules.RemoveRange(plan.Schedules);
        var itemMap = planItems.ToDictionary(i => i.ProductId, i => i.Id);
        foreach (var schReq in request.Schedules)
        {
            var sch = _mapper.Map<DetailedProductionPlanSchedule>(schReq);
            sch.PlanId = plan.Id;
            if (itemMap.TryGetValue(sch.ProductId, out var planItemId))
            {
                sch.PlanItemId = planItemId;
            }
            sch.ScheduleDate = schReq.ScheduleDate.Date;

            var key = $"{sch.ProductId}_{sch.ScheduleDate:yyyyMMdd}_{sch.ShiftName}";
            if (existingOrdersBySchedule.TryGetValue(key, out var ord))
            {
                sch.ProductionOrderId = ord.ProductionOrderId;
                sch.ProductionOrderNo = ord.ProductionOrderNo;
            }

            sch.CreatedBy = currentUser;
            sch.CreatedDate = DateTime.UtcNow;
            sch.IsDeleted = false;
            _context.DetailedProductionPlanSchedules.Add(sch);
        }

        await _context.SaveChangesAsync();

        if (plan.MasterPlanId.HasValue)
        {
            await SyncMasterPlanAllocatedQuantitiesAsync(plan.MasterPlanId.Value, currentUser);
        }

        var result = await GetByIdAsync(id);
        return result!;
    }

    public async Task<bool> DeleteAsync(ulong id, string currentUser)
    {
        var plan = await _context.DetailedProductionPlans
            .Include(p => p.Items)
            .Include(p => p.Schedules)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null) return false;

        plan.IsDeleted = true;
        plan.UpdatedBy = currentUser;
        plan.UpdatedDate = DateTime.UtcNow;

        foreach (var item in plan.Items)
        {
            item.IsDeleted = true;
            item.UpdatedBy = currentUser;
            item.UpdatedDate = DateTime.UtcNow;
        }

        foreach (var sch in plan.Schedules)
        {
            sch.IsDeleted = true;
            sch.UpdatedBy = currentUser;
            sch.UpdatedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        if (plan.MasterPlanId.HasValue)
        {
            await SyncMasterPlanAllocatedQuantitiesAsync(plan.MasterPlanId.Value, currentUser);
        }

        return true;
    }

    public async Task<DetailedProductionPlanDto> ChangeStatusAsync(ulong id, int newStatus, string currentUser, string? remark = null)
    {
        var plan = await _context.DetailedProductionPlans.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (plan == null)
            throw new NotFoundException($"Không tìm thấy Kế hoạch chi tiết #{id}");

        plan.Status = newStatus;
        if (!string.IsNullOrWhiteSpace(remark))
        {
            plan.Remark = string.IsNullOrWhiteSpace(plan.Remark) ? remark : $"{plan.Remark}\n[{DateTime.UtcNow:dd/MM/yyyy HH:mm}] {remark}";
        }
        plan.UpdatedBy = currentUser;
        plan.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        var result = await GetByIdAsync(id);
        return result!;
    }

    public async Task<List<MasterProductionPlanDto>> GetMasterPlansAsync()
    {
        var plans = await _context.MasterProductionPlans
            .Include(p => p.Items)
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

        return _mapper.Map<List<MasterProductionPlanDto>>(plans);
    }

    public async Task<MasterProductionPlanDto?> GetMasterPlanByIdAsync(ulong masterPlanId)
    {
        var plan = await _context.MasterProductionPlans
            .Include(p => p.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == masterPlanId && !p.IsDeleted);

        if (plan == null) return null;
        return _mapper.Map<MasterProductionPlanDto>(plan);
    }

    #region Allocation Algorithm (Forward & Backward)

    public PlanAllocationCalculationResultDto CalculateAllocation(PlanAllocationCalculationRequest request)
    {
        if (request.FromDate.Date > request.ToDate.Date)
        {
            return new PlanAllocationCalculationResultDto
            {
                IsFullyAllocated = false,
                Message = "Từ ngày không thể lớn hơn Đến ngày"
            };
        }

        var method = (request.Method ?? "FORWARD").Trim().ToUpperInvariant();
        var periodType = (request.PeriodType ?? "DAY").Trim().ToUpperInvariant();

        // 1. Generate all dates in period
        var allDates = new List<DateTime>();
        for (var d = request.FromDate.Date; d <= request.ToDate.Date; d = d.AddDays(1))
        {
            allDates.Add(d);
        }

        // Build day-off lookup
        var dayOffMap = request.DayOffOverrides?.ToDictionary(x => x.Date.Date, x => x) ?? new Dictionary<DateTime, DayOffOverride>();

        // Default shifts if period type is SHIFT
        var shifts = request.Shifts != null && request.Shifts.Count > 0
            ? request.Shifts
            : new List<string> { "Ca 1", "Ca 2", "Ca 3" };

        var generatedSchedules = new List<DetailedProductionPlanScheduleDto>();
        var itemSummaries = new List<PlanItemAllocatedSummaryDto>();

        // Sort dates depending on Forward vs Backward
        var scheduleDates = method == "BACKWARD"
            ? allDates.OrderByDescending(d => d).ToList()
            : allDates.OrderBy(d => d).ToList();

        foreach (var item in request.Items)
        {
            decimal remainingNeeded = item.RequiredQuantity;
            decimal totalAllocated = 0;
            var itemSchedules = new List<DetailedProductionPlanScheduleDto>();

            var dailyCap = item.DailyCapacity > 0 ? item.DailyCapacity : 100m;

            if (periodType == "SHIFT")
            {
                decimal shiftCap = Math.Round(dailyCap / Math.Max(1, shifts.Count), 4);

                foreach (var date in scheduleDates)
                {
                    bool isDayOff = false;
                    bool isOvertime = false;

                    if (dayOffMap.TryGetValue(date, out var overrideInfo))
                    {
                        isDayOff = overrideInfo.IsDayOff;
                        isOvertime = overrideInfo.IsOvertime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        isDayOff = true;
                    }

                    foreach (var shiftName in (method == "BACKWARD" ? shifts.AsEnumerable().Reverse().ToList() : shifts))
                    {
                        decimal alloc = 0;
                        if (!isDayOff || isOvertime)
                        {
                            if (remainingNeeded > 0)
                            {
                                alloc = Math.Min(remainingNeeded, shiftCap);
                                remainingNeeded -= alloc;
                                totalAllocated += alloc;
                            }
                        }

                        itemSchedules.Add(new DetailedProductionPlanScheduleDto
                        {
                            ProductId = item.ProductId,
                            ScheduleDate = date,
                            ShiftName = shiftName,
                            Capacity = shiftCap,
                            PlannedQuantity = alloc,
                            IsDayOff = isDayOff,
                            IsOvertime = isOvertime
                        });
                    }
                }
            }
            else // DAY mode
            {
                foreach (var date in scheduleDates)
                {
                    bool isDayOff = false;
                    bool isOvertime = false;

                    if (dayOffMap.TryGetValue(date, out var overrideInfo))
                    {
                        isDayOff = overrideInfo.IsDayOff;
                        isOvertime = overrideInfo.IsOvertime;
                    }
                    else if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        isDayOff = true;
                    }

                    decimal alloc = 0;
                    if (!isDayOff || isOvertime)
                    {
                        if (remainingNeeded > 0)
                        {
                            alloc = Math.Min(remainingNeeded, dailyCap);
                            remainingNeeded -= alloc;
                            totalAllocated += alloc;
                        }
                    }

                    itemSchedules.Add(new DetailedProductionPlanScheduleDto
                    {
                        ProductId = item.ProductId,
                        ScheduleDate = date,
                        ShiftName = null,
                        Capacity = dailyCap,
                        PlannedQuantity = alloc,
                        IsDayOff = isDayOff,
                        IsOvertime = isOvertime
                    });
                }
            }

            // Always order schedule chronologically for display
            var orderedItemSchedules = itemSchedules
                .OrderBy(s => s.ScheduleDate)
                .ThenBy(s => s.ShiftName)
                .ToList();

            generatedSchedules.AddRange(orderedItemSchedules);

            itemSummaries.Add(new PlanItemAllocatedSummaryDto
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                RequiredQuantity = item.RequiredQuantity,
                AllocatedQuantity = totalAllocated,
                RemainingQuantity = Math.Max(0, item.RequiredQuantity - totalAllocated)
            });
        }

        bool allMatched = itemSummaries.All(x => x.IsMatched);
        string message = allMatched
            ? $"Phân bổ thành công theo phương pháp {(method == "BACKWARD" ? "Lùi" : "Tiến")}. Tất cả sản phẩm đã khớp 100% nhu cầu."
            : $"Phân bổ hoàn tất. Còn {itemSummaries.Count(x => !x.IsMatched)} sản phẩm chưa phân bổ đủ công suất trong kỳ.";

        return new PlanAllocationCalculationResultDto
        {
            Schedules = generatedSchedules,
            ItemSummaries = itemSummaries,
            IsFullyAllocated = allMatched,
            Message = message
        };
    }

    #endregion

    #region Quantity Validation

    public ValidateQuantitiesResultDto ValidateQuantities(ValidateQuantitiesRequest request)
    {
        var plannedByProduct = request.Schedules
            .GroupBy(s => s.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.PlannedQuantity));

        var details = new List<QuantityComparisonItemDto>();
        bool allMatch = true;

        foreach (var item in request.Items)
        {
            plannedByProduct.TryGetValue(item.ProductId, out var planned);
            var comp = new QuantityComparisonItemDto
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                RequiredQuantity = item.RequiredQuantity,
                PlannedQuantity = planned
            };
            if (comp.Difference != 0) allMatch = false;
            details.Add(comp);
        }

        string summary = allMatch
            ? "Tất cả sản phẩm khớp 100% nhu cầu sản xuất!"
            : $"Có {details.Count(d => d.Difference != 0)} sản phẩm có số lượng kế hoạch chênh lệch so với nhu cầu.";

        return new ValidateQuantitiesResultDto
        {
            IsValid = allMatch,
            SummaryMessage = summary,
            Details = details
        };
    }

    #endregion

    #region Production Order Generation (MRP)

    public async Task<GenerateOrdersResultDto> GenerateProductionOrdersAsync(GenerateOrdersFromPlanRequest request, string currentUser)
    {
        var plan = await _context.DetailedProductionPlans
            .Include(p => p.Items)
            .Include(p => p.Schedules)
            .FirstOrDefaultAsync(p => p.Id == request.PlanId && !p.IsDeleted);

        if (plan == null)
            throw new NotFoundException($"Không tìm thấy Kế hoạch chi tiết #{request.PlanId}");

        var candidateSchedules = plan.Schedules
            .Where(s => s.PlannedQuantity > 0 && !s.IsDeleted)
            .Where(s => request.ProductIds == null || request.ProductIds.Count == 0 || request.ProductIds.Contains(s.ProductId))
            .ToList();

        if (candidateSchedules.Count == 0)
        {
            return new GenerateOrdersResultDto
            {
                GeneratedCount = 0,
                Message = "Không có phân bổ sản xuất nào có số lượng > 0 để lập lệnh sản xuất."
            };
        }

        var productIds = candidateSchedules.Select(s => s.ProductId).Distinct().ToList();
        var products = await _context.Products.AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        // Preload active BOM and Routings
        var boms = await _context.Boms.AsNoTracking()
            .Where(b => productIds.Contains((ulong)b.ProductId) && !b.IsDeleted)
            .GroupBy(b => (ulong)b.ProductId)
            .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(b => b.CreatedDate).First());

        var routings = await _context.Routings.AsNoTracking()
            .Where(r => productIds.Contains((ulong)r.ProductId) && !r.IsDeleted)
            .GroupBy(r => (ulong)r.ProductId)
            .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(r => r.CreatedDate).First());

        var generatedItems = new List<GeneratedOrderItemDto>();
        int orderIndex = 1;

        if (request.GroupBy.Equals("ITEM", StringComparison.OrdinalIgnoreCase))
        {
            // Group by Product for whole plan period
            var itemGroups = candidateSchedules.GroupBy(s => s.ProductId).ToList();
            foreach (var group in itemGroups)
            {
                var productId = group.Key;
                products.TryGetValue(productId, out var prod);

                var totalQty = group.Sum(s => s.PlannedQuantity);
                var minDate = group.Min(s => s.ScheduleDate);
                var maxDate = group.Max(s => s.ScheduleDate);

                var orderNo = await GenerateProductionOrderNoAsync(orderIndex++);
                ulong? bomId = boms.TryGetValue(productId, out var bom) ? bom.Id : null;
                ulong? routingId = routings.TryGetValue(productId, out var rt) ? rt.Id : null;

                var prodOrder = new ProductionOrder
                {
                    OrderNo = orderNo,
                    OrderType = 3, // MRP (Kế hoạch chi tiết)
                    ProductId = productId,
                    UnitId = prod?.DefaultUnitId.HasValue == true ? (ulong)prod.DefaultUnitId.Value : null,
                    BomId = bomId,
                    RoutingId = routingId,
                    LineId = plan.LineId,
                    PlannedQuantity = totalQty,
                    CompletedQuantity = 0,
                    ScrapQuantity = 0,
                    PlannedStartDate = minDate,
                    PlannedEndDate = maxDate,
                    Status = request.AutoApprove ? 2 : 1, // 1=Draft, 2=Approved
                    SalesOrderNo = plan.PlanCode,
                    BatchNo = $"{plan.PlanCode}-{minDate:yyMMdd}",
                    Description = $"Sinh từ Kế hoạch chi tiết {plan.PlanCode} ({plan.PlanName})",
                    Remark = request.Remark,
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.ProductionOrders.Add(prodOrder);
                await _context.SaveChangesAsync();

                // Auto-create BOM materials and operations if BOM exists
                if (bomId.HasValue)
                {
                    await PopulateBomMaterialsAsync(prodOrder.Id, bomId.Value, totalQty, currentUser);
                }
                if (routingId.HasValue)
                {
                    await PopulateRoutingOperationsAsync(prodOrder.Id, routingId.Value, currentUser);
                }

                // Link back to all schedules of this product
                foreach (var sch in group)
                {
                    sch.ProductionOrderId = prodOrder.Id;
                    sch.ProductionOrderNo = prodOrder.OrderNo;
                    sch.UpdatedBy = currentUser;
                    sch.UpdatedDate = DateTime.UtcNow;
                }

                generatedItems.Add(new GeneratedOrderItemDto
                {
                    OrderId = prodOrder.Id,
                    OrderNo = prodOrder.OrderNo,
                    ProductId = productId,
                    ProductCode = prod?.ProductCode ?? "",
                    ProductName = prod?.ProductName ?? "",
                    PlannedQuantity = totalQty,
                    PlannedStartDate = minDate,
                    PlannedEndDate = maxDate,
                    Status = request.AutoApprove ? "Approved" : "Draft"
                });
            }
        }
        else // Group by DAY and PRODUCT
        {
            var dayGroups = candidateSchedules.GroupBy(s => new { s.ScheduleDate, s.ProductId }).ToList();
            foreach (var group in dayGroups)
            {
                var productId = group.Key.ProductId;
                var schDate = group.Key.ScheduleDate;
                products.TryGetValue(productId, out var prod);

                var totalQty = group.Sum(s => s.PlannedQuantity);
                var orderNo = await GenerateProductionOrderNoAsync(orderIndex++);
                ulong? bomId = boms.TryGetValue(productId, out var bom) ? bom.Id : null;
                ulong? routingId = routings.TryGetValue(productId, out var rt) ? rt.Id : null;

                var prodOrder = new ProductionOrder
                {
                    OrderNo = orderNo,
                    OrderType = 3, // MRP (Kế hoạch chi tiết)
                    ProductId = productId,
                    UnitId = prod?.DefaultUnitId.HasValue == true ? (ulong)prod.DefaultUnitId.Value : null,
                    BomId = bomId,
                    RoutingId = routingId,
                    LineId = plan.LineId,
                    PlannedQuantity = totalQty,
                    CompletedQuantity = 0,
                    ScrapQuantity = 0,
                    PlannedStartDate = schDate,
                    PlannedEndDate = schDate,
                    Status = request.AutoApprove ? 2 : 1, // 1=Draft, 2=Approved
                    SalesOrderNo = plan.PlanCode,
                    BatchNo = $"{plan.PlanCode}-{schDate:yyyyMMdd}",
                    Description = $"Sinh từ Kế hoạch chi tiết {plan.PlanCode} ngày {schDate:dd/MM/yyyy}",
                    Remark = request.Remark,
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.ProductionOrders.Add(prodOrder);
                await _context.SaveChangesAsync();

                if (bomId.HasValue)
                {
                    await PopulateBomMaterialsAsync(prodOrder.Id, bomId.Value, totalQty, currentUser);
                }
                if (routingId.HasValue)
                {
                    await PopulateRoutingOperationsAsync(prodOrder.Id, routingId.Value, currentUser);
                }

                // Link back to schedule items
                foreach (var sch in group)
                {
                    sch.ProductionOrderId = prodOrder.Id;
                    sch.ProductionOrderNo = prodOrder.OrderNo;
                    sch.UpdatedBy = currentUser;
                    sch.UpdatedDate = DateTime.UtcNow;
                }

                generatedItems.Add(new GeneratedOrderItemDto
                {
                    OrderId = prodOrder.Id,
                    OrderNo = prodOrder.OrderNo,
                    ProductId = productId,
                    ProductCode = prod?.ProductCode ?? "",
                    ProductName = prod?.ProductName ?? "",
                    PlannedQuantity = totalQty,
                    PlannedStartDate = schDate,
                    PlannedEndDate = schDate,
                    Status = request.AutoApprove ? "Approved" : "Draft"
                });
            }
        }

        // Update plan summary
        plan.ProductionOrderCount = await _context.DetailedProductionPlanSchedules
            .Where(s => s.PlanId == plan.Id && s.ProductionOrderId.HasValue && !s.IsDeleted)
            .Select(s => s.ProductionOrderId)
            .Distinct()
            .CountAsync();

        if (plan.Status == 1) // Switch from Draft to InProgress
        {
            plan.Status = 3;
        }
        plan.UpdatedBy = currentUser;
        plan.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new GenerateOrdersResultDto
        {
            GeneratedCount = generatedItems.Count,
            GeneratedOrders = generatedItems,
            Message = $"Đã lập thành công {generatedItems.Count} lệnh sản xuất từ kế hoạch chi tiết {plan.PlanCode}."
        };
    }

    private async Task PopulateBomMaterialsAsync(ulong productionOrderId, ulong bomId, decimal plannedQuantity, string currentUser)
    {
        var details = await _context.BomDetails.AsNoTracking()
            .Where(d => d.BomId == (long)bomId && !d.IsDeleted)
            .ToListAsync();

        if (details.Count == 0) return;

        var productIds = details.Where(d => d.ProductId.HasValue).Select(d => d.ProductId!.Value).Distinct().ToList();
        var products = await _context.Products.AsNoTracking()
            .Where(p => productIds.Contains((long)p.Id))
            .ToDictionaryAsync(p => (long)p.Id, p => p);

        var units = await _context.Units.AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.UnitName);

        var warehouses = await _context.Warehouses.AsNoTracking()
            .ToDictionaryAsync(w => w.Id, w => w.WarehouseName);

        foreach (var d in details)
        {
            if (!d.ProductId.HasValue) continue;

            var norm = d.Quantity ?? 0m;
            var reqQty = Math.Round(norm * plannedQuantity, 4);
            products.TryGetValue(d.ProductId.Value, out var prod);
            var unitId = d.UnitId.HasValue ? (ulong)d.UnitId.Value : (prod?.DefaultUnitId.HasValue == true ? (ulong)prod.DefaultUnitId.Value : (ulong?)null);
            var unitName = unitId.HasValue && units.TryGetValue(unitId.Value, out var u) ? u : null;
            var whId = prod?.DefaultWarehouseId.HasValue == true ? (ulong)prod.DefaultWarehouseId.Value : (ulong?)null;
            var whName = whId.HasValue && warehouses.TryGetValue(whId.Value, out var w) ? w : null;

            var mat = new ProductionOrderMaterial
            {
                ProductionOrderId = productionOrderId,
                ProductId = (ulong)d.ProductId.Value,
                ProductCode = prod?.ProductCode,
                ProductName = prod?.ProductName,
                UnitId = unitId,
                UnitName = unitName,
                WarehouseId = whId,
                WarehouseName = whName,
                NormQuantity = norm,
                RequiredQuantity = reqQty,
                IssuedQuantity = 0,
                CurrentStock = 0,
                Remark = null,
                CreatedBy = currentUser,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.ProductionOrderMaterials.Add(mat);
        }

        await _context.SaveChangesAsync();
    }

    private async Task PopulateRoutingOperationsAsync(ulong productionOrderId, ulong routingId, string currentUser)
    {
        var ops = await _context.RoutingOperations.AsNoTracking()
            .Where(o => o.RoutingId == (long)routingId && !o.IsDeleted)
            .OrderBy(o => o.Sequence)
            .ToListAsync();

        if (ops.Count == 0) return;

        int seq = 1;
        foreach (var op in ops)
        {
            var ordOp = new ProductionOrderOperation
            {
                ProductionOrderId = productionOrderId,
                Sequence = op.Sequence ?? seq++,
                OperationCode = op.RoutingOperationCode ?? $"OP-{seq}",
                OperationName = op.RoutingOperationName ?? $"Công đoạn {seq}",
                WorkCenterId = null,
                MachineId = null,
                Status = 1, // NotStarted
                Remark = op.Description,
                CreatedBy = currentUser,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.ProductionOrderOperations.Add(ordOp);
        }

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Helper Methods

    private async Task<string> GeneratePlanCodeAsync()
    {
        var prefix = $"KHCT-{DateTime.UtcNow:yyyyMM}";
        var lastCode = await _context.DetailedProductionPlans
            .Where(p => p.PlanCode.StartsWith(prefix))
            .OrderByDescending(p => p.PlanCode)
            .Select(p => p.PlanCode)
            .FirstOrDefaultAsync();

        if (lastCode == null)
            return $"{prefix}-001";

        var parts = lastCode.Split('-');
        if (parts.Length >= 3 && int.TryParse(parts[2], out var seq))
        {
            return $"{prefix}-{(seq + 1):D3}";
        }

        return $"{prefix}-{Guid.NewGuid().ToString()[..4].ToUpperInvariant()}";
    }

    private async Task<string> GenerateProductionOrderNoAsync(int seedOffset = 0)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"LSX-{today}";
        var countToday = await _context.ProductionOrders
            .Where(x => x.OrderNo.StartsWith(prefix))
            .CountAsync();

        return $"{prefix}-{(countToday + seedOffset):D4}";
    }

    private async Task SyncMasterPlanAllocatedQuantitiesAsync(ulong masterPlanId, string currentUser)
    {
        var masterPlan = await _context.MasterProductionPlans
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == masterPlanId && !m.IsDeleted);

        if (masterPlan == null) return;

        // Sum allocated quantities from all active detailed plans referencing this master plan
        var allocationsByProduct = await _context.DetailedProductionPlanItems
            .Where(i => !i.IsDeleted)
            .Where(i => _context.DetailedProductionPlans.Any(p => p.Id == i.PlanId && p.MasterPlanId == masterPlanId && !p.IsDeleted && p.Status != 5))
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, TotalAllocated = g.Sum(x => x.PlannedQuantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.TotalAllocated);

        foreach (var item in masterPlan.Items)
        {
            allocationsByProduct.TryGetValue(item.ProductId, out var alloc);
            item.AllocatedQuantity = alloc;
            item.RemainingQuantity = Math.Max(0, item.PlannedQuantity - alloc);
            item.UpdatedBy = currentUser;
            item.UpdatedDate = DateTime.UtcNow;
        }

        masterPlan.UpdatedBy = currentUser;
        masterPlan.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    #endregion
}
