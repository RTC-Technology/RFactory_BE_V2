using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.QrCode.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.QrCode.Services;

public class QrCodeService : IQrCodeService
{
    private readonly RFactoryContext _db;

    public QrCodeService(RFactoryContext db)
    {
        _db = db;
    }

    public Task<Result<QrCodeDto>> GenerateAsync(QrCodeGenerateRequest request, CancellationToken ct = default)
    {
        var entityType = (request.EntityType ?? "CUSTOM").ToUpperInvariant();
        var code = request.Code?.Trim() ?? string.Empty;
        var id = request.EntityId;

        var payload = BuildPayload(entityType, id, code);

        var dto = new QrCodeDto
        {
            Payload = payload,
            EntityType = entityType,
            EntityId = id,
            Code = code,
            LabelTitle = !string.IsNullOrWhiteSpace(request.Title) ? request.Title : $"{entityType} #{code}",
            LabelSubtitle = request.Subtitle ?? $"ID: {id?.ToString() ?? "N/A"} | RF-MES",
            GeneratedAt = DateTime.UtcNow
        };

        return Task.FromResult(Result<QrCodeDto>.Success(dto));
    }

    public async Task<Result<QrScanResultDto>> ScanAndLookupAsync(string rawPayload, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawPayload))
        {
            return Result<QrScanResultDto>.Failure("Payload is empty.");
        }

        var parsed = ParsePayload(rawPayload);
        var result = new QrScanResultDto
        {
            EntityType = parsed.EntityType,
            EntityId = parsed.EntityId,
            Code = parsed.Code,
            Found = false
        };

        switch (parsed.EntityType)
        {
            case "PRODUCT":
                await LookupProductAsync(parsed, result, ct);
                break;
            case "LOCATION":
            case "WAREHOUSE_LOCATION":
                await LookupLocationAsync(parsed, result, ct);
                break;
            case "MACHINE":
                await LookupMachineAsync(parsed, result, ct);
                break;
            case "EMPLOYEE":
                await LookupEmployeeAsync(parsed, result, ct);
                break;
            case "GOODS_RECEIPT":
                await LookupGoodsReceiptAsync(parsed, result, ct);
                break;
            case "INVENTORY":
                await LookupInventoryAsync(parsed, result, ct);
                break;
            default:
                // Fallback for custom or unknown formats
                result.Found = true;
                result.Title = parsed.Code;
                result.Subtitle = $"Custom Payload: {rawPayload}";
                result.Status = "ACTIVE";
                result.Attributes["RawPayload"] = rawPayload;
                result.ActionRoute = "/qr-code";
                break;
        }

        return Result<QrScanResultDto>.Success(result);
    }

    public async Task<Result<List<QrItemLookupDto>>> GetAvailableItemsAsync(string entityType, CancellationToken ct = default)
    {
        var type = (entityType ?? string.Empty).ToUpperInvariant();
        var list = new List<QrItemLookupDto>();

        switch (type)
        {
            case "PRODUCT":
                list = await _db.Products
                    .Where(p => p.IsDeleted != 1)
                    .Select(p => new QrItemLookupDto
                    {
                        Id = p.Id,
                        EntityType = "PRODUCT",
                        Code = p.ProductCode,
                        Name = p.ProductName,
                        Description = p.Specification ?? string.Empty,
                        Payload = $"RF:PRODUCT:{p.Id}:{p.ProductCode}"
                    })
                    .Take(100)
                    .ToListAsync(ct);
                break;

            case "LOCATION":
            case "WAREHOUSE_LOCATION":
                list = await _db.WarehouseLocations
                    .Where(l => l.IsDeleted != 1)
                    .Select(l => new QrItemLookupDto
                    {
                        Id = l.Id,
                        EntityType = "LOCATION",
                        Code = l.LocationCode,
                        Name = l.LocationName,
                        Description = $"Zone ID: {l.ZoneId}",
                        Payload = $"RF:LOCATION:{l.Id}:{l.LocationCode}"
                    })
                    .Take(100)
                    .ToListAsync(ct);
                break;

            case "MACHINE":
                list = await _db.Machines
                    .Where(m => m.IsDeleted != 1)
                    .Select(m => new QrItemLookupDto
                    {
                        Id = m.Id,
                        EntityType = "MACHINE",
                        Code = m.MachineCode,
                        Name = m.MachineName,
                        Description = m.Model ?? string.Empty,
                        Payload = $"RF:MACHINE:{m.Id}:{m.MachineCode}"
                    })
                    .Take(100)
                    .ToListAsync(ct);
                break;

            case "EMPLOYEE":
                list = await _db.Employees
                    .Where(e => e.IsDeleted != 1)
                    .Select(e => new QrItemLookupDto
                    {
                        Id = e.Id,
                        EntityType = "EMPLOYEE",
                        Code = e.EmployeeCode,
                        Name = e.FullName,
                        Description = e.Position ?? string.Empty,
                        Payload = $"RF:EMPLOYEE:{e.Id}:{e.EmployeeCode}"
                    })
                    .Take(100)
                    .ToListAsync(ct);
                break;

            case "GOODS_RECEIPT":
                list = await _db.GoodsReceipts
                    .Where(g => g.IsDeleted != 1)
                    .Select(g => new QrItemLookupDto
                    {
                        Id = g.Id,
                        EntityType = "GOODS_RECEIPT",
                        Code = g.ReceiptNo,
                        Name = $"Receipt {g.ReceiptNo}",
                        Description = $"Date: {g.ReceiptDate:yyyy-MM-dd}",
                        Payload = $"RF:GOODS_RECEIPT:{g.Id}:{g.ReceiptNo}"
                    })
                    .Take(100)
                    .ToListAsync(ct);
                break;

            case "INVENTORY":
                list = await _db.Inventories
                    .Where(i => i.IsDeleted != 1)
                    .Select(i => new QrItemLookupDto
                    {
                        Id = i.Id,
                        EntityType = "INVENTORY",
                        Code = $"INV-{i.Id}",
                        Name = $"Stock Item #{i.Id}",
                        Description = $"Qty: {i.QuantityOnHand}",
                        Payload = $"RF:INVENTORY:{i.Id}:INV-{i.Id}"
                    })
                    .Take(100)
                    .ToListAsync(ct);
                break;
        }

        return Result<List<QrItemLookupDto>>.Success(list);
    }

    public async Task<Result<List<QrCodeDto>>> BatchGenerateAsync(QrBatchPrintRequest request, CancellationToken ct = default)
    {
        var itemsResult = await GetAvailableItemsAsync(request.EntityType, ct);
        if (!itemsResult.IsSuccess) return Result<List<QrCodeDto>>.Failure(itemsResult.Error);

        var targetIds = new HashSet<ulong>(request.EntityIds ?? new List<ulong>());
        var selected = targetIds.Any()
            ? itemsResult.Data.Where(x => targetIds.Contains(x.Id)).ToList()
            : itemsResult.Data;

        var result = selected.Select(item => new QrCodeDto
        {
            Payload = item.Payload,
            EntityType = item.EntityType,
            EntityId = item.Id,
            Code = item.Code,
            LabelTitle = item.Name,
            LabelSubtitle = $"{item.EntityType} | Code: {item.Code}",
            GeneratedAt = DateTime.UtcNow
        }).ToList();

        return Result<List<QrCodeDto>>.Success(result);
    }

    // ─── Helpers & Lookups ──────────────────────────────────────────────────

    private static string BuildPayload(string entityType, ulong? id, string code)
    {
        return $"RF:{entityType}:{id?.ToString() ?? "0"}:{code}";
    }

    private static (string EntityType, ulong? EntityId, string Code) ParsePayload(string rawPayload)
    {
        var parts = rawPayload.Trim().Split(':');
        if (parts.Length >= 4 && parts[0] == "RF")
        {
            ulong.TryParse(parts[2], out var parsedId);
            return (parts[1].ToUpperInvariant(), parsedId > 0 ? parsedId : null, parts[3]);
        }

        if (parts.Length == 2)
        {
            return (parts[0].ToUpperInvariant(), null, parts[1]);
        }

        return ("CUSTOM", null, rawPayload);
    }

    private async Task LookupProductAsync((string EntityType, ulong? EntityId, string Code) p, QrScanResultDto res, CancellationToken ct)
    {
        var prod = await _db.Products
            .FirstOrDefaultAsync(x => (p.EntityId.HasValue && x.Id == p.EntityId.Value) || x.ProductCode == p.Code, ct);

        if (prod != null)
        {
            res.Found = true;
            res.EntityId = prod.Id;
            res.Code = prod.ProductCode;
            res.Title = prod.ProductName;
            res.Subtitle = $"Spec: {prod.Specification ?? "N/A"}";
            res.Status = prod.IsActive == 1 ? "ACTIVE" : "INACTIVE";
            res.Location = "Catalog Master";
            res.Attributes["ProductCode"] = prod.ProductCode;
            res.Attributes["MinStockLevel"] = prod.MinStockLevel?.ToString() ?? "N/A";
            res.Attributes["MaxStockLevel"] = prod.MaxStockLevel?.ToString() ?? "N/A";
            res.ActionRoute = "/products";
        }
    }

    private async Task LookupLocationAsync((string EntityType, ulong? EntityId, string Code) p, QrScanResultDto res, CancellationToken ct)
    {
        var loc = await _db.WarehouseLocations
            .FirstOrDefaultAsync(x => (p.EntityId.HasValue && x.Id == p.EntityId.Value) || x.LocationCode == p.Code, ct);

        if (loc != null)
        {
            res.Found = true;
            res.EntityId = loc.Id;
            res.Code = loc.LocationCode;
            res.Title = loc.LocationName;
            res.Subtitle = $"Barcode: {loc.Barcode ?? loc.LocationCode}";
            res.Status = loc.IsActive == 1 ? "ACTIVE" : "INACTIVE";
            res.Location = $"Zone #{loc.ZoneId}";
            res.Attributes["LocationCode"] = loc.LocationCode;
            res.Attributes["Barcode"] = loc.Barcode ?? "N/A";
            res.Attributes["MaxCapacity"] = loc.MaxCapacity?.ToString() ?? "N/A";
            res.ActionRoute = "/warehouses";
        }
    }

    private async Task LookupMachineAsync((string EntityType, ulong? EntityId, string Code) p, QrScanResultDto res, CancellationToken ct)
    {
        var mac = await _db.Machines
            .FirstOrDefaultAsync(x => (p.EntityId.HasValue && x.Id == p.EntityId.Value) || x.MachineCode == p.Code, ct);

        if (mac != null)
        {
            res.Found = true;
            res.EntityId = mac.Id;
            res.Code = mac.MachineCode;
            res.Title = mac.MachineName;
            res.Subtitle = $"Model: {mac.Model ?? "N/A"} | Serial: {mac.SerialNumber ?? "N/A"}";
            res.Status = mac.Status ?? "NORMAL";
            res.Location = $"Line #{mac.LineId?.ToString() ?? "N/A"}";
            res.Attributes["MachineCode"] = mac.MachineCode;
            res.Attributes["IPAddress"] = mac.IpAddress ?? "N/A";
            res.ActionRoute = "/machines";
        }
    }

    private async Task LookupEmployeeAsync((string EntityType, ulong? EntityId, string Code) p, QrScanResultDto res, CancellationToken ct)
    {
        var emp = await _db.Employees
            .FirstOrDefaultAsync(x => (p.EntityId.HasValue && x.Id == p.EntityId.Value) || x.EmployeeCode == p.Code, ct);

        if (emp != null)
        {
            res.Found = true;
            res.EntityId = emp.Id;
            res.Code = emp.EmployeeCode;
            res.Title = emp.FullName;
            res.Subtitle = $"Position: {emp.Position ?? "Staff"}";
            res.Status = emp.Status == 1 ? "ACTIVE" : "INACTIVE";
            res.Location = emp.Email ?? "N/A";
            res.Attributes["EmployeeCode"] = emp.EmployeeCode;
            res.Attributes["Phone"] = emp.Phone ?? "N/A";
            res.ActionRoute = "/employees";
        }
    }

    private async Task LookupGoodsReceiptAsync((string EntityType, ulong? EntityId, string Code) p, QrScanResultDto res, CancellationToken ct)
    {
        var gr = await _db.GoodsReceipts
            .FirstOrDefaultAsync(x => (p.EntityId.HasValue && x.Id == p.EntityId.Value) || x.ReceiptNo == p.Code, ct);

        if (gr != null)
        {
            res.Found = true;
            res.EntityId = gr.Id;
            res.Code = gr.ReceiptNo;
            res.Title = $"Goods Receipt #{gr.ReceiptNo}";
            res.Subtitle = $"Date: {gr.ReceiptDate:yyyy-MM-dd HH:mm}";
            res.Status = gr.Status ?? "DRAFT";
            res.Location = $"Warehouse #{gr.WarehouseId}";
            res.Attributes["ReceiptNo"] = gr.ReceiptNo;
            res.Attributes["SupplierId"] = gr.SupplierId.ToString();
            res.ActionRoute = "/goods-receipt";
        }
    }

    private async Task LookupInventoryAsync((string EntityType, ulong? EntityId, string Code) p, QrScanResultDto res, CancellationToken ct)
    {
        var inv = await _db.Inventories
            .FirstOrDefaultAsync(x => p.EntityId.HasValue && x.Id == p.EntityId.Value, ct);

        if (inv != null)
        {
            res.Found = true;
            res.EntityId = inv.Id;
            res.Code = $"INV-{inv.Id}";
            res.Title = $"Inventory Stock Item #{inv.Id}";
            res.Subtitle = $"Product ID: {inv.ProductId}";
            res.Status = "IN_STOCK";
            res.Location = $"Warehouse #{inv.WarehouseId} / Location #{inv.LocationId}";
            res.Attributes["QuantityOnHand"] = inv.QuantityOnHand.ToString();
            res.Attributes["AllocatedQuantity"] = inv.AllocatedQuantity.ToString();
            res.ActionRoute = "/inventory";
        }
    }
}
