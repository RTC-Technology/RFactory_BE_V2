using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RFactory.Shared.Exceptions;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Services;

public class ProductionOrderService : IProductionOrderService
{
    private readonly RFactoryContext _context;
    private readonly IMapper _mapper;

    public ProductionOrderService(RFactoryContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductionOrderDto>> GetAllAsync(ProductionOrderFilterDto? filter = null)
    {
        var query = _context.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted);

        if (filter != null)
        {
            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(x => x.Priority == filter.Priority.Value);

            if (filter.OrderType.HasValue)
                query = query.Where(x => x.OrderType == filter.OrderType.Value);

            if (filter.ProductId.HasValue)
                query = query.Where(x => x.ProductId == filter.ProductId.Value);

            if (filter.LineId.HasValue)
                query = query.Where(x => x.LineId == filter.LineId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.PlannedStartDate >= filter.FromDate.Value || x.CreatedDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(x => x.PlannedEndDate <= filter.ToDate.Value || x.CreatedDate <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var kw = filter.SearchKeyword.Trim();
                query = query.Where(x => x.OrderNo.Contains(kw) ||
                                         (x.Description != null && x.Description.Contains(kw)) ||
                                         (x.BatchNo != null && x.BatchNo.Contains(kw)) ||
                                         (x.SalesOrderNo != null && x.SalesOrderNo.Contains(kw)));
            }
        }

        var orders = await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        if (orders.Count == 0) return new List<ProductionOrderDto>();

        // Preload reference data for enrichment
        var productIds = orders.Select(x => (long)x.ProductId).Distinct().ToList();
        var products = await _context.Products.AsNoTracking()
            .Where(p => productIds.Contains((long)p.Id))
            .ToDictionaryAsync(p => p.Id, p => new { p.ProductCode, p.ProductName });

        var bomIds = orders.Where(x => x.BomId.HasValue).Select(x => (ulong)x.BomId!.Value).Distinct().ToList();
        var boms = await _context.Boms.AsNoTracking()
            .Where(b => bomIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, b => new { b.BomCode, b.BomName, b.Version });

        var routingIds = orders.Where(x => x.RoutingId.HasValue).Select(x => (ulong)x.RoutingId!.Value).Distinct().ToList();
        var routings = await _context.Routings.AsNoTracking()
            .Where(r => routingIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Version);

        var lineIds = orders.Where(x => x.LineId.HasValue).Select(x => (ulong)x.LineId!.Value).Distinct().ToList();
        var lines = await _context.Lines.AsNoTracking()
            .Where(l => lineIds.Contains(l.Id))
            .ToDictionaryAsync(l => l.Id, l => new { l.LineCode, l.LineName });

        var workCenterIds = orders.Where(x => x.WorkCenterId.HasValue).Select(x => (ulong)x.WorkCenterId!.Value).Distinct().ToList();
        var workCenters = await _context.WorkCenters.AsNoTracking()
            .Where(w => workCenterIds.Contains(w.Id))
            .ToDictionaryAsync(w => w.Id, w => w.WorkCenterName);

        var customerIds = orders.Where(x => x.CustomerId.HasValue).Select(x => (ulong)x.CustomerId!.Value).Distinct().ToList();
        var customers = await _context.Customers.AsNoTracking()
            .Where(c => customerIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.CustomerName);

        var unitIds = orders.Where(x => x.UnitId.HasValue).Select(x => (ulong)x.UnitId!.Value).Distinct().ToList();
        var units = await _context.Units.AsNoTracking()
            .Where(u => unitIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UnitName);

        var factoryIds = orders.Where(x => x.FactoryId.HasValue).Select(x => (ulong)x.FactoryId!.Value).Distinct().ToList();
        var factories = await _context.Factories.AsNoTracking()
            .Where(f => factoryIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, f => f.FactoryName);

        var result = new List<ProductionOrderDto>(orders.Count);
        foreach (var order in orders)
        {
            var dto = _mapper.Map<ProductionOrderDto>(order);

            if (products.TryGetValue(order.ProductId, out var prod))
            {
                dto.ProductCode = prod.ProductCode;
                dto.ProductName = prod.ProductName;
            }

            if (order.BomId.HasValue && boms.TryGetValue(order.BomId.Value, out var bom))
            {
                dto.BomCode = bom.BomCode;
                dto.BomName = bom.BomName;
                dto.BomVersion = bom.Version;
            }

            if (order.RoutingId.HasValue && routings.TryGetValue(order.RoutingId.Value, out var rVer))
            {
                dto.RoutingVersion = rVer;
            }

            if (order.LineId.HasValue && lines.TryGetValue(order.LineId.Value, out var line))
            {
                dto.LineCode = line.LineCode;
                dto.LineName = line.LineName;
            }

            if (order.WorkCenterId.HasValue && workCenters.TryGetValue(order.WorkCenterId.Value, out var wcName))
            {
                dto.WorkCenterName = wcName;
            }

            if (order.CustomerId.HasValue && customers.TryGetValue(order.CustomerId.Value, out var custName))
            {
                dto.CustomerName = custName;
            }

            if (order.UnitId.HasValue && units.TryGetValue(order.UnitId.Value, out var uName))
            {
                dto.UnitName = uName;
            }

            if (order.FactoryId.HasValue && factories.TryGetValue(order.FactoryId.Value, out var fName))
            {
                dto.FactoryName = fName;
            }

            result.Add(dto);
        }

        return result;
    }

    public async Task<ProductionOrderDto?> GetByIdAsync(ulong id)
    {
        var order = await _context.ProductionOrders.AsNoTracking()
            .Include(x => x.Materials.Where(m => !m.IsDeleted))
            .Include(x => x.Operations.Where(o => !o.IsDeleted).OrderBy(o => o.Sequence))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (order == null) return null;

        var dto = _mapper.Map<ProductionOrderDto>(order);
        await EnrichDtoAsync(dto, order);

        // Enrich Materials with latest Stock from Inventory
        if (dto.Materials.Count > 0)
        {
            var matProductIds = dto.Materials.Select(m => (long)m.ProductId).Distinct().ToList();
            var stockMap = await GetCurrentStockMapAsync(matProductIds);

            foreach (var m in dto.Materials)
            {
                if (stockMap.TryGetValue((long)m.ProductId, out var stock))
                {
                    m.CurrentStock = stock;
                }
            }
        }

        return dto;
    }

    public async Task<List<ProductionOrderMaterialDto>> PreviewBomMaterialsAsync(ulong bomId, decimal plannedQuantity)
    {
        if (plannedQuantity <= 0) plannedQuantity = 1;

        var bomDetails = await _context.BomDetails.AsNoTracking()
            .Where(b => b.BomId == (long)bomId && !b.IsDeleted)
            .ToListAsync();

        if (bomDetails.Count == 0) return new List<ProductionOrderMaterialDto>();

        var productIds = bomDetails.Where(b => b.ProductId.HasValue).Select(b => b.ProductId!.Value).Distinct().ToList();
        var products = await _context.Products.AsNoTracking()
            .Where(p => productIds.Contains((long)p.Id))
            .ToDictionaryAsync(p => (long)p.Id, p => new { p.ProductCode, p.ProductName, p.DefaultUnitId, p.DefaultWarehouseId });

        var unitIds = bomDetails.Where(b => b.UnitId.HasValue).Select(b => (ulong)b.UnitId!.Value).Distinct().ToList();
        var units = await _context.Units.AsNoTracking()
            .Where(u => unitIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UnitName);

        var warehouseIds = products.Values.Where(p => p.DefaultWarehouseId.HasValue).Select(p => (ulong)p.DefaultWarehouseId!.Value).Distinct().ToList();
        var warehouses = await _context.Warehouses.AsNoTracking()
            .Where(w => warehouseIds.Contains(w.Id))
            .ToDictionaryAsync(w => w.Id, w => w.WarehouseName);

        var stockMap = await GetCurrentStockMapAsync(productIds);

        var result = new List<ProductionOrderMaterialDto>();
        foreach (var d in bomDetails)
        {
            if (!d.ProductId.HasValue) continue;

            var norm = d.Quantity ?? 0m;
            // SL theo định mức = Định mức x SL sản phẩm cần sản xuất
            var reqQty = Math.Round(norm * plannedQuantity, 4);

            var item = new ProductionOrderMaterialDto
            {
                ProductId = (ulong)d.ProductId.Value,
                NormQuantity = norm,
                RequiredQuantity = reqQty,
                IssuedQuantity = 0,
                UnitId = d.UnitId.HasValue ? (ulong)d.UnitId.Value : null,
                IsSubstituted = false
            };

            if (products.TryGetValue(d.ProductId.Value, out var prod))
            {
                item.ProductCode = prod.ProductCode;
                item.ProductName = prod.ProductName;

                if (!item.UnitId.HasValue && prod.DefaultUnitId.HasValue)
                {
                    item.UnitId = (ulong)prod.DefaultUnitId.Value;
                }

                if (prod.DefaultWarehouseId.HasValue)
                {
                    item.WarehouseId = (ulong)prod.DefaultWarehouseId.Value;
                    if (warehouses.TryGetValue(item.WarehouseId.Value, out var whName))
                    {
                        item.WarehouseName = whName;
                    }
                }
            }

            if (item.UnitId.HasValue && units.TryGetValue(item.UnitId.Value, out var uName))
            {
                item.UnitName = uName;
            }

            if (stockMap.TryGetValue(d.ProductId.Value, out var stock))
            {
                item.CurrentStock = stock;
            }

            result.Add(item);
        }

        return result;
    }

    public async Task<List<ProductionOrderOperationDto>> PreviewRoutingOperationsAsync(ulong routingId)
    {
        var routingOps = await _context.RoutingOperations.AsNoTracking()
            .Where(r => r.RoutingId == (long)routingId && !r.IsDeleted)
            .OrderBy(r => r.Sequence)
            .ToListAsync();

        if (routingOps.Count == 0) return new List<ProductionOrderOperationDto>();

        return routingOps.Select((ro, index) => new ProductionOrderOperationDto
        {
            RoutingOperationId = ro.Id,
            Sequence = ro.Sequence ?? (index + 1),
            OperationCode = ro.RoutingOperationCode ?? $"OP-{(index + 1):D2}",
            OperationName = ro.RoutingOperationName ?? $"Công đoạn {index + 1}",
            Remark = ro.Description,
            Status = 1
        }).ToList();
    }

    public async Task<ProductionOrderDto> CreateAsync(ProductionOrderCreateRequest request, string currentUser)
    {
        if (request.PlannedQuantity <= 0)
            throw new BusinessException("Planned quantity must be greater than 0");

        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId && !p.IsDeleted);
        if (!productExists)
            throw new NotFoundException($"Product with ID {request.ProductId} does not exist");

        var orderNo = string.IsNullOrWhiteSpace(request.OrderNo)
            ? await GenerateOrderNoAsync()
            : request.OrderNo.Trim();

        var codeExists = await _context.ProductionOrders.AnyAsync(x => x.OrderNo == orderNo && !x.IsDeleted);
        if (codeExists)
            throw new BusinessException($"Production order with code '{orderNo}' already exists", 409);

        // Infer UnitId from Product if not provided
        ulong? unitId = request.UnitId;
        if (!unitId.HasValue)
        {
            var prod = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (prod?.DefaultUnitId.HasValue == true)
            {
                unitId = (ulong)prod.DefaultUnitId.Value;
            }
        }

        var order = _mapper.Map<ProductionOrder>(request);
        order.OrderNo = orderNo;
        order.OrderType = request.OrderType > 0 ? request.OrderType : 4;
        order.UnitId = unitId;
        order.Status = 1; // Draft
        order.CompletedQuantity = 0;
        order.ScrapQuantity = 0;
        order.CreatedBy = currentUser;
        order.CreatedDate = DateTime.UtcNow;
        order.IsDeleted = false;

        _context.ProductionOrders.Add(order);
        await _context.SaveChangesAsync();

        // ── Process Materials ──
        if (request.Materials != null && request.Materials.Count > 0)
        {
            foreach (var m in request.Materials)
            {
                var mat = _mapper.Map<ProductionOrderMaterial>(m);
                mat.ProductionOrderId = order.Id;
                mat.CreatedBy = currentUser;
                mat.CreatedDate = DateTime.UtcNow;
                mat.IsDeleted = false;
                _context.ProductionOrderMaterials.Add(mat);
            }
        }
        else if (request.BomId.HasValue)
        {
            // Auto-populate from BOM
            var bomMaterials = await PreviewBomMaterialsAsync(request.BomId.Value, request.PlannedQuantity);
            foreach (var bm in bomMaterials)
            {
                var mat = new ProductionOrderMaterial
                {
                    ProductionOrderId = order.Id,
                    ProductId = bm.ProductId,
                    ProductCode = bm.ProductCode,
                    ProductName = bm.ProductName,
                    UnitId = bm.UnitId,
                    UnitName = bm.UnitName,
                    NormQuantity = bm.NormQuantity,
                    RequiredQuantity = bm.RequiredQuantity,
                    IssuedQuantity = 0,
                    CurrentStock = bm.CurrentStock,
                    WarehouseId = bm.WarehouseId,
                    WarehouseName = bm.WarehouseName,
                    IsSubstituted = bm.IsSubstituted,
                    Remark = bm.Remark,
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                _context.ProductionOrderMaterials.Add(mat);
            }
        }

        // ── Process Operations ──
        if (request.Operations != null && request.Operations.Count > 0)
        {
            int seq = 1;
            foreach (var o in request.Operations)
            {
                var op = _mapper.Map<ProductionOrderOperation>(o);
                op.ProductionOrderId = order.Id;
                if (op.Sequence <= 0) op.Sequence = seq++;
                op.CreatedBy = currentUser;
                op.CreatedDate = DateTime.UtcNow;
                op.IsDeleted = false;
                _context.ProductionOrderOperations.Add(op);
            }
        }
        else if (request.RoutingId.HasValue)
        {
            // Auto-populate from Routing
            var routingOps = await PreviewRoutingOperationsAsync(request.RoutingId.Value);
            foreach (var ro in routingOps)
            {
                var op = new ProductionOrderOperation
                {
                    ProductionOrderId = order.Id,
                    RoutingOperationId = ro.RoutingOperationId,
                    Sequence = ro.Sequence,
                    OperationCode = ro.OperationCode,
                    OperationName = ro.OperationName,
                    Status = 1,
                    Remark = ro.Remark,
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                _context.ProductionOrderOperations.Add(op);
            }
        }

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(order.Id))!;
    }

    public async Task<ProductionOrderDto> UpdateAsync(ulong id, ProductionOrderUpdateRequest request, string currentUser)
    {
        var order = await _context.ProductionOrders
            .Include(x => x.Materials.Where(m => !m.IsDeleted))
            .Include(x => x.Operations.Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (order == null)
            throw new NotFoundException($"Production order with ID {id} not found");

        if (order.Status == 4 || order.Status == 5 || order.Status == 6)
            throw new BusinessException("Cannot edit a completed, cancelled, or closed production order");

        if (request.PlannedQuantity <= 0)
            throw new BusinessException("Planned quantity must be greater than 0");

        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId && !p.IsDeleted);
        if (!productExists)
            throw new NotFoundException($"Product with ID {request.ProductId} does not exist");

        if (request.OrderType.HasValue)
            order.OrderType = request.OrderType.Value;

        order.Description = request.Description?.Trim();
        order.ProductId = request.ProductId;
        order.UnitId = request.UnitId;
        order.BomId = request.BomId;
        order.RoutingId = request.RoutingId;
        order.FactoryId = request.FactoryId;
        order.LineId = request.LineId;
        order.WorkCenterId = request.WorkCenterId;
        order.CustomerId = request.CustomerId;
        order.PlannedQuantity = request.PlannedQuantity;

        if (request.CompletedQuantity.HasValue)
            order.CompletedQuantity = request.CompletedQuantity.Value;
        if (request.ScrapQuantity.HasValue)
            order.ScrapQuantity = request.ScrapQuantity.Value;

        order.PlannedStartDate = request.PlannedStartDate;
        order.PlannedEndDate = request.PlannedEndDate;
        order.ActualStartDate = request.ActualStartDate;
        order.ActualEndDate = request.ActualEndDate;
        order.Priority = request.Priority;
        order.BatchNo = request.BatchNo?.Trim();
        order.SalesOrderNo = request.SalesOrderNo?.Trim();
        order.Remark = request.Remark?.Trim();

        order.UpdatedBy = currentUser;
        order.UpdatedDate = DateTime.UtcNow;

        // ── Synchronize Materials if provided ──
        if (request.Materials != null)
        {
            var existingMats = order.Materials.ToList();
            var incomingMatIds = request.Materials.Where(m => m.Id.HasValue && m.Id.Value > 0).Select(m => m.Id!.Value).ToHashSet();

            // Soft delete removed materials
            foreach (var em in existingMats)
            {
                if (!incomingMatIds.Contains(em.Id))
                {
                    em.IsDeleted = true;
                    em.UpdatedBy = currentUser;
                    em.UpdatedDate = DateTime.UtcNow;
                }
            }

            // Update or add materials
            foreach (var im in request.Materials)
            {
                if (im.Id.HasValue && im.Id.Value > 0)
                {
                    var existing = existingMats.FirstOrDefault(x => x.Id == im.Id.Value);
                    if (existing != null)
                    {
                        existing.ProductId = im.ProductId;
                        existing.ProductCode = im.ProductCode;
                        existing.ProductName = im.ProductName;
                        existing.UnitId = im.UnitId;
                        existing.UnitName = im.UnitName;
                        existing.NormQuantity = im.NormQuantity;
                        existing.RequiredQuantity = im.RequiredQuantity;
                        if (im.IssuedQuantity.HasValue) existing.IssuedQuantity = im.IssuedQuantity.Value;
                        existing.WarehouseId = im.WarehouseId;
                        existing.WarehouseName = im.WarehouseName;
                        existing.IsSubstituted = im.IsSubstituted;
                        existing.OriginalProductId = im.OriginalProductId;
                        existing.Remark = im.Remark;
                        existing.UpdatedBy = currentUser;
                        existing.UpdatedDate = DateTime.UtcNow;
                    }
                }
                else
                {
                    var newMat = _mapper.Map<ProductionOrderMaterial>(im);
                    newMat.ProductionOrderId = order.Id;
                    newMat.CreatedBy = currentUser;
                    newMat.CreatedDate = DateTime.UtcNow;
                    newMat.IsDeleted = false;
                    _context.ProductionOrderMaterials.Add(newMat);
                }
            }
        }

        // ── Synchronize Operations if provided ──
        if (request.Operations != null)
        {
            var existingOps = order.Operations.ToList();
            var incomingOpIds = request.Operations.Where(o => o.Id.HasValue && o.Id.Value > 0).Select(o => o.Id!.Value).ToHashSet();

            foreach (var eo in existingOps)
            {
                if (!incomingOpIds.Contains(eo.Id))
                {
                    eo.IsDeleted = true;
                    eo.UpdatedBy = currentUser;
                    eo.UpdatedDate = DateTime.UtcNow;
                }
            }

            int seq = 1;
            foreach (var io in request.Operations)
            {
                if (io.Id.HasValue && io.Id.Value > 0)
                {
                    var existing = existingOps.FirstOrDefault(x => x.Id == io.Id.Value);
                    if (existing != null)
                    {
                        existing.RoutingOperationId = io.RoutingOperationId;
                        existing.Sequence = io.Sequence > 0 ? io.Sequence : seq++;
                        existing.OperationCode = io.OperationCode;
                        existing.OperationName = io.OperationName;
                        existing.WorkCenterId = io.WorkCenterId;
                        existing.WorkCenterName = io.WorkCenterName;
                        existing.MachineId = io.MachineId;
                        existing.MachineName = io.MachineName;
                        existing.AssignedEmployeeId = io.AssignedEmployeeId;
                        existing.AssignedEmployeeName = io.AssignedEmployeeName;
                        existing.PlannedStartDate = io.PlannedStartDate;
                        existing.PlannedEndDate = io.PlannedEndDate;
                        existing.Status = io.Status;
                        existing.Remark = io.Remark;
                        existing.UpdatedBy = currentUser;
                        existing.UpdatedDate = DateTime.UtcNow;
                    }
                }
                else
                {
                    var newOp = _mapper.Map<ProductionOrderOperation>(io);
                    newOp.ProductionOrderId = order.Id;
                    if (newOp.Sequence <= 0) newOp.Sequence = seq++;
                    newOp.CreatedBy = currentUser;
                    newOp.CreatedDate = DateTime.UtcNow;
                    newOp.IsDeleted = false;
                    _context.ProductionOrderOperations.Add(newOp);
                }
            }
        }

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(order.Id))!;
    }

    public async Task<ProductionOrderDto> ResetMaterialsToBomAsync(ulong orderId, string currentUser)
    {
        var order = await _context.ProductionOrders
            .Include(x => x.Materials.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);

        if (order == null)
            throw new NotFoundException($"Production order with ID {orderId} not found");

        if (!order.BomId.HasValue)
            throw new BusinessException("Lệnh sản xuất chưa được gán định mức BOM");

        // Soft delete all existing materials
        foreach (var m in order.Materials)
        {
            m.IsDeleted = true;
            m.UpdatedBy = currentUser;
            m.UpdatedDate = DateTime.UtcNow;
        }

        // Re-generate from BOM
        var bomMaterials = await PreviewBomMaterialsAsync(order.BomId.Value, order.PlannedQuantity);
        foreach (var bm in bomMaterials)
        {
            var mat = new ProductionOrderMaterial
            {
                ProductionOrderId = order.Id,
                ProductId = bm.ProductId,
                ProductCode = bm.ProductCode,
                ProductName = bm.ProductName,
                UnitId = bm.UnitId,
                UnitName = bm.UnitName,
                NormQuantity = bm.NormQuantity,
                RequiredQuantity = bm.RequiredQuantity,
                IssuedQuantity = 0,
                CurrentStock = bm.CurrentStock,
                WarehouseId = bm.WarehouseId,
                WarehouseName = bm.WarehouseName,
                IsSubstituted = false,
                Remark = bm.Remark,
                CreatedBy = currentUser,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.ProductionOrderMaterials.Add(mat);
        }

        order.UpdatedBy = currentUser;
        order.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(order.Id))!;
    }

    public async Task<ProductionOrderDto> DuplicateAsync(ulong id, string currentUser)
    {
        var source = await _context.ProductionOrders.AsNoTracking()
            .Include(x => x.Materials.Where(m => !m.IsDeleted))
            .Include(x => x.Operations.Where(o => !o.IsDeleted).OrderBy(o => o.Sequence))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (source == null)
            throw new NotFoundException($"Production order with ID {id} not found");

        var newOrderNo = await GenerateOrderNoAsync();

        var newOrder = new ProductionOrder
        {
            OrderNo = newOrderNo,
            OrderType = source.OrderType,
            Description = $"[Nhân bản từ {source.OrderNo}] {source.Description}".Trim(),
            ProductId = source.ProductId,
            UnitId = source.UnitId,
            BomId = source.BomId,
            RoutingId = source.RoutingId,
            FactoryId = source.FactoryId,
            LineId = source.LineId,
            WorkCenterId = source.WorkCenterId,
            CustomerId = source.CustomerId,
            PlannedQuantity = source.PlannedQuantity,
            CompletedQuantity = 0,
            ScrapQuantity = 0,
            PlannedStartDate = source.PlannedStartDate,
            PlannedEndDate = source.PlannedEndDate,
            Priority = source.Priority,
            Status = 1, // Draft
            BatchNo = source.BatchNo,
            SalesOrderNo = source.SalesOrderNo,
            Remark = source.Remark,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.ProductionOrders.Add(newOrder);
        await _context.SaveChangesAsync();

        // Duplicate materials
        foreach (var m in source.Materials)
        {
            var newMat = new ProductionOrderMaterial
            {
                ProductionOrderId = newOrder.Id,
                ProductId = m.ProductId,
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                UnitId = m.UnitId,
                UnitName = m.UnitName,
                NormQuantity = m.NormQuantity,
                RequiredQuantity = m.RequiredQuantity,
                IssuedQuantity = 0,
                CurrentStock = m.CurrentStock,
                WarehouseId = m.WarehouseId,
                WarehouseName = m.WarehouseName,
                IsSubstituted = m.IsSubstituted,
                OriginalProductId = m.OriginalProductId,
                Remark = m.Remark,
                CreatedBy = currentUser,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.ProductionOrderMaterials.Add(newMat);
        }

        // Duplicate operations
        foreach (var o in source.Operations)
        {
            var newOp = new ProductionOrderOperation
            {
                ProductionOrderId = newOrder.Id,
                RoutingOperationId = o.RoutingOperationId,
                Sequence = o.Sequence,
                OperationCode = o.OperationCode,
                OperationName = o.OperationName,
                WorkCenterId = o.WorkCenterId,
                WorkCenterName = o.WorkCenterName,
                MachineId = o.MachineId,
                MachineName = o.MachineName,
                AssignedEmployeeId = o.AssignedEmployeeId,
                AssignedEmployeeName = o.AssignedEmployeeName,
                PlannedStartDate = o.PlannedStartDate,
                PlannedEndDate = o.PlannedEndDate,
                Status = 1, // Pending
                Remark = o.Remark,
                CreatedBy = currentUser,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.ProductionOrderOperations.Add(newOp);
        }

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(newOrder.Id))!;
    }

    public async Task<bool> DeleteAsync(ulong id, string currentUser)
    {
        var order = await _context.ProductionOrders
            .Include(x => x.Materials.Where(m => !m.IsDeleted))
            .Include(x => x.Operations.Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (order == null)
            throw new NotFoundException($"Production order with ID {id} not found");

        if (order.Status == 3) // InProgress
            throw new BusinessException("Không thể xóa lệnh sản xuất đang thực hiện. Hãy tạm dừng hoặc hủy lệnh trước.");

        order.IsDeleted = true;
        order.UpdatedBy = currentUser;
        order.UpdatedDate = DateTime.UtcNow;

        foreach (var m in order.Materials)
        {
            m.IsDeleted = true;
            m.UpdatedBy = currentUser;
            m.UpdatedDate = DateTime.UtcNow;
        }

        foreach (var o in order.Operations)
        {
            o.IsDeleted = true;
            o.UpdatedBy = currentUser;
            o.UpdatedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProductionOrderDto> ChangeStatusAsync(ulong id, int newStatus, string currentUser, string? remark = null)
    {
        var order = await _context.ProductionOrders.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (order == null)
            throw new NotFoundException($"Production order with ID {id} not found");

        // Status: 1=Draft, 2=Released, 3=InProgress, 4=Completed, 5=Cancelled, 6=Closed, 7=Paused
        if (newStatus < 1 || newStatus > 7)
            throw new BusinessException("Invalid production order status");

        order.Status = newStatus;

        if (newStatus == 3 && !order.ActualStartDate.HasValue) // InProgress
        {
            order.ActualStartDate = DateTime.UtcNow;
        }
        else if ((newStatus == 4 || newStatus == 6) && !order.ActualEndDate.HasValue) // Completed or Closed
        {
            order.ActualEndDate = DateTime.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(remark))
        {
            order.Remark = string.IsNullOrWhiteSpace(order.Remark)
                ? $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {remark.Trim()}"
                : $"{order.Remark}\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {remark.Trim()}";
        }

        order.UpdatedBy = currentUser;
        order.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(order.Id))!;
    }

    public async Task<ProductionOrderDto> RecordProgressAsync(ulong id, ProductionOrderProgressRequest request, string currentUser)
    {
        var order = await _context.ProductionOrders.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (order == null)
            throw new NotFoundException($"Production order with ID {id} not found");

        if (order.Status == 5 || order.Status == 6)
            throw new BusinessException("Cannot record progress on a cancelled or closed production order");

        if (request.CompletedQuantityDelta < 0 || request.ScrapQuantityDelta < 0)
            throw new BusinessException("Progress delta quantities cannot be negative");

        order.CompletedQuantity += request.CompletedQuantityDelta;
        order.ScrapQuantity += request.ScrapQuantityDelta;

        if (!string.IsNullOrWhiteSpace(request.BatchNo))
            order.BatchNo = request.BatchNo.Trim();

        // Auto move from Released (2) to InProgress (3) upon recording progress
        if (order.Status == 1 || order.Status == 2)
        {
            order.Status = 3;
            if (!order.ActualStartDate.HasValue)
                order.ActualStartDate = DateTime.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(request.Remark))
        {
            order.Remark = string.IsNullOrWhiteSpace(order.Remark)
                ? $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] Progress: +{request.CompletedQuantityDelta} ok, +{request.ScrapQuantityDelta} scrap. {request.Remark.Trim()}"
                : $"{order.Remark}\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] Progress: +{request.CompletedQuantityDelta} ok, +{request.ScrapQuantityDelta} scrap. {request.Remark.Trim()}";
        }

        order.UpdatedBy = currentUser;
        order.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(order.Id))!;
    }

    private async Task<Dictionary<long, decimal>> GetCurrentStockMapAsync(List<long> productIds)
    {
        if (productIds.Count == 0) return new Dictionary<long, decimal>();

        var inventories = await _context.Inventories.AsNoTracking()
            .Where(inv => inv.ProductId.HasValue && productIds.Contains(inv.ProductId.Value) && !inv.IsDeleted)
            .GroupBy(inv => inv.ProductId!.Value)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQty = g.Sum(x => (x.Quantity ?? 0m) - (x.ReservedQuantity ?? 0m))
            })
            .ToListAsync();

        return inventories.ToDictionary(x => x.ProductId, x => x.TotalQty);
    }

    private async Task<string> GenerateOrderNoAsync()
    {
        var prefix = $"PO-{DateTime.UtcNow:yyyyMM}";
        var count = await _context.ProductionOrders
            .CountAsync(x => x.OrderNo.StartsWith(prefix));

        var seq = count + 1;
        var candidate = $"{prefix}-{seq:D4}";

        while (await _context.ProductionOrders.AnyAsync(x => x.OrderNo == candidate))
        {
            seq++;
            candidate = $"{prefix}-{seq:D4}";
        }

        return candidate;
    }

    private async Task EnrichDtoAsync(ProductionOrderDto dto, ProductionOrder order)
    {
        var prod = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == order.ProductId);
        if (prod != null)
        {
            dto.ProductCode = prod.ProductCode;
            dto.ProductName = prod.ProductName;
        }

        if (order.BomId.HasValue)
        {
            var bom = await _context.Boms.AsNoTracking().FirstOrDefaultAsync(b => b.Id == order.BomId.Value);
            if (bom != null)
            {
                dto.BomCode = bom.BomCode;
                dto.BomName = bom.BomName;
                dto.BomVersion = bom.Version;
            }
        }

        if (order.RoutingId.HasValue)
        {
            var routing = await _context.Routings.AsNoTracking().FirstOrDefaultAsync(r => r.Id == order.RoutingId.Value);
            if (routing != null)
                dto.RoutingVersion = routing.Version;
        }

        if (order.LineId.HasValue)
        {
            var line = await _context.Lines.AsNoTracking().FirstOrDefaultAsync(l => l.Id == order.LineId.Value);
            if (line != null)
            {
                dto.LineCode = line.LineCode;
                dto.LineName = line.LineName;
            }
        }

        if (order.WorkCenterId.HasValue)
        {
            var wc = await _context.WorkCenters.AsNoTracking().FirstOrDefaultAsync(w => w.Id == order.WorkCenterId.Value);
            if (wc != null)
                dto.WorkCenterName = wc.WorkCenterName;
        }

        if (order.CustomerId.HasValue)
        {
            var cust = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == order.CustomerId.Value);
            if (cust != null)
                dto.CustomerName = cust.CustomerName;
        }

        if (order.UnitId.HasValue)
        {
            var unit = await _context.Units.AsNoTracking().FirstOrDefaultAsync(u => u.Id == order.UnitId.Value);
            if (unit != null)
                dto.UnitName = unit.UnitName;
        }

        if (order.FactoryId.HasValue)
        {
            var factory = await _context.Factories.AsNoTracking().FirstOrDefaultAsync(f => f.Id == order.FactoryId.Value);
            if (factory != null)
                dto.FactoryName = factory.FactoryName;
        }
    }
}
