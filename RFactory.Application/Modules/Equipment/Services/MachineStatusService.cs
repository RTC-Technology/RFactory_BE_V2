using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Equipment.Services;

public class MachineStatusService : IMachineStatusService
{
    private readonly RFactoryContext _db;

    public MachineStatusService(RFactoryContext db)
    {
        _db = db;
    }

    #region Reason Codes

    public async Task<List<MachineReasonCodeDto>> GetAllReasonsAsync(string? reasonType = null, CancellationToken ct = default)
    {
        var query = _db.MachineReasonCodes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(reasonType))
        {
            var type = reasonType.Trim().ToUpperInvariant();
            query = query.Where(r => r.ReasonType == type);
        }

        var list = await query.OrderBy(r => r.Category).ThenBy(r => r.ReasonCode).ToListAsync(ct);

        return list.Select(MapReasonDto).ToList();
    }

    public async Task<MachineReasonCodeDto?> GetReasonByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.MachineReasonCodes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);
        return entity == null ? null : MapReasonDto(entity);
    }

    public async Task<Result<MachineReasonCodeDto>> CreateReasonAsync(MachineReasonCodeRequest request, CancellationToken ct = default)
    {
        var code = request.ReasonCode?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<MachineReasonCodeDto>.Failure("Reason code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ReasonName))
        {
            return Result<MachineReasonCodeDto>.Failure("Reason name is required.");
        }

        var exists = await _db.MachineReasonCodes.AnyAsync(r => r.ReasonCode == code, ct);
        if (exists)
        {
            return Result<MachineReasonCodeDto>.Failure($"Reason code '{code}' already exists.");
        }

        var entity = new MachineReasonCode
        {
            ReasonCode = code,
            ReasonName = request.ReasonName.Trim(),
            ReasonType = (request.ReasonType ?? "DOWNTIME").ToUpperInvariant(),
            Category = string.IsNullOrWhiteSpace(request.Category) ? "General" : request.Category.Trim(),
            IsPlanned = request.IsPlanned,
            IsActive = request.IsActive,
            Description = request.Description?.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        _db.MachineReasonCodes.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result<MachineReasonCodeDto>.Success(MapReasonDto(entity));
    }

    public async Task<Result<MachineReasonCodeDto>> UpdateReasonAsync(ulong id, MachineReasonCodeRequest request, CancellationToken ct = default)
    {
        var entity = await _db.MachineReasonCodes.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (entity == null)
        {
            return Result<MachineReasonCodeDto>.Failure($"Reason code #{id} not found.");
        }

        var code = request.ReasonCode?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<MachineReasonCodeDto>.Failure("Reason code is required.");
        }

        var clash = await _db.MachineReasonCodes.AnyAsync(r => r.ReasonCode == code && r.Id != id, ct);
        if (clash)
        {
            return Result<MachineReasonCodeDto>.Failure($"Reason code '{code}' already exists.");
        }

        entity.ReasonCode = code;
        entity.ReasonName = request.ReasonName.Trim();
        entity.ReasonType = (request.ReasonType ?? entity.ReasonType).ToUpperInvariant();
        entity.Category = string.IsNullOrWhiteSpace(request.Category) ? entity.Category : request.Category.Trim();
        entity.IsPlanned = request.IsPlanned;
        entity.IsActive = request.IsActive;
        entity.Description = request.Description?.Trim();
        entity.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Result<MachineReasonCodeDto>.Success(MapReasonDto(entity));
    }

    public async Task<Result<bool>> DeleteReasonAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _db.MachineReasonCodes.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (entity == null)
        {
            return Result<bool>.Failure($"Reason code #{id} not found.");
        }

        entity.IsDeleted = true;
        entity.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    #endregion

    #region Realtime Status Monitoring

    public async Task<List<MachineRealtimeStatusDto>> GetRealtimeStatusesAsync(long? lineId = null, CancellationToken ct = default)
    {
        var machinesQuery = _db.Machines.AsNoTracking();
        if (lineId.HasValue)
        {
            machinesQuery = machinesQuery.Where(m => m.LineId == lineId.Value);
        }

        var machines = await machinesQuery.OrderBy(m => m.MachineCode).ToListAsync(ct);
        if (!machines.Any())
        {
            return new List<MachineRealtimeStatusDto>();
        }

        var machineIds = machines.Select(m => m.Id).ToList();
        var lines = await _db.Lines.AsNoTracking().ToDictionaryAsync(l => (long)l.Id, l => l.LineName, ct);
        var machineTypes = await _db.MachineTypes.AsNoTracking().ToDictionaryAsync(mt => (long)mt.Id, mt => mt.MachineTypeName, ct);

        // Fetch latest active/open status or most recent status per machine
        var activeHistories = await _db.MachineStatusHistories
            .AsNoTracking()
            .Where(h => machineIds.Contains(h.MachineId))
            .GroupBy(h => h.MachineId)
            .Select(g => g.OrderByDescending(h => h.StartTime).First())
            .ToListAsync(ct);

        var historyMap = activeHistories.ToDictionary(h => h.MachineId, h => h);

        // Today's summary per machine
        var todayStart = DateTime.UtcNow.Date;
        var todayDowntimes = await _db.MachineStatusHistories
            .AsNoTracking()
            .Where(h => machineIds.Contains(h.MachineId) && h.StartTime >= todayStart && h.Status == "DOWNTIME")
            .GroupBy(h => h.MachineId)
            .Select(g => new { MachineId = g.Key, TotalSeconds = g.Sum(x => x.DurationSeconds ?? 0) })
            .ToDictionaryAsync(x => x.MachineId, x => x.TotalSeconds, ct);

        var todayDefects = await _db.MachineDefectLogs
            .AsNoTracking()
            .Where(d => machineIds.Contains(d.MachineId) && d.LoggedTime >= todayStart)
            .GroupBy(d => new { d.MachineId, d.DefectType })
            .Select(g => new { g.Key.MachineId, g.Key.DefectType, TotalQty = g.Sum(x => x.Quantity) })
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        var results = new List<MachineRealtimeStatusDto>();

        foreach (var m in machines)
        {
            historyMap.TryGetValue(m.Id, out var curHist);

            var lineName = m.LineId.HasValue && lines.TryGetValue(m.LineId.Value, out var ln) ? ln : string.Empty;
            var typeName = m.MachineTypeId.HasValue && machineTypes.TryGetValue(m.MachineTypeId.Value, out var tn) ? tn : string.Empty;

            var status = curHist?.Status ?? (m.Status == 1 ? "RUNNING" : "IDLE");
            var startTime = curHist?.StartTime ?? m.CreatedDate;
            var durationSeconds = curHist?.EndTime.HasValue == true
                ? (curHist.DurationSeconds ?? 0)
                : (long)Math.Max(0, (now - startTime).TotalSeconds);

            todayDowntimes.TryGetValue(m.Id, out var todayDtSec);
            if (status == "DOWNTIME" && curHist?.EndTime == null)
            {
                todayDtSec += durationSeconds;
            }

            var scrapQty = todayDefects.FirstOrDefault(d => d.MachineId == m.Id && d.DefectType == "SCRAP")?.TotalQty ?? 0m;
            var reworkQty = todayDefects.FirstOrDefault(d => d.MachineId == m.Id && d.DefectType == "REWORK")?.TotalQty ?? 0m;

            results.Add(new MachineRealtimeStatusDto
            {
                MachineId = m.Id,
                MachineCode = m.MachineCode,
                MachineName = m.MachineName,
                LineId = m.LineId,
                LineName = lineName,
                MachineTypeId = m.MachineTypeId,
                MachineTypeName = typeName,
                Status = status,
                StatusStartTime = startTime,
                DurationSeconds = durationSeconds,
                DurationMinutes = Math.Round(durationSeconds / 60.0, 1),
                CurrentReasonId = curHist?.ReasonId,
                CurrentReasonCode = curHist?.ReasonCode,
                CurrentReasonName = curHist?.ReasonName,
                OperatorName = curHist?.OperatorName,
                WorkOrderCode = curHist?.WorkOrderCode,
                Remarks = curHist?.Remarks,
                TodayDowntimeMinutes = Math.Round(todayDtSec / 60.0, 1),
                TodayScrapQty = scrapQty,
                TodayReworkQty = reworkQty
            });
        }

        return results;
    }

    public async Task<MachineRealtimeStatusDto?> GetMachineStatusAsync(ulong machineId, CancellationToken ct = default)
    {
        var list = await GetRealtimeStatusesAsync(null, ct);
        return list.FirstOrDefault(m => m.MachineId == machineId);
    }

    public async Task<MachineStatusDashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken ct = default)
    {
        var statuses = await GetRealtimeStatusesAsync(null, ct);

        var total = statuses.Count;
        var running = statuses.Count(s => s.Status == "RUNNING");
        var idle = statuses.Count(s => s.Status == "IDLE");
        var downtime = statuses.Count(s => s.Status == "DOWNTIME");
        var changeover = statuses.Count(s => s.Status == "CHANGEOVER");
        var maintenance = statuses.Count(s => s.Status == "MAINTENANCE");
        var offline = statuses.Count(s => s.Status == "OFFLINE");

        var productiveOrAvailable = running + idle + changeover;
        var availability = total > 0 ? Math.Round((double)productiveOrAvailable / total * 100.0, 1) : 0.0;

        return new MachineStatusDashboardSummaryDto
        {
            TotalMachines = total,
            RunningCount = running,
            IdleCount = idle,
            DowntimeCount = downtime,
            ChangeoverCount = changeover,
            MaintenanceCount = maintenance,
            OfflineCount = offline,
            AvailabilityPercentage = availability,
            TotalDowntimeMinutesToday = Math.Round(statuses.Sum(s => s.TodayDowntimeMinutes), 1),
            TotalScrapQtyToday = statuses.Sum(s => s.TodayScrapQty),
            TotalReworkQtyToday = statuses.Sum(s => s.TodayReworkQty)
        };
    }

    public async Task<Result<MachineRealtimeStatusDto>> ChangeStatusAsync(ChangeMachineStatusRequest request, CancellationToken ct = default)
    {
        var machine = await _db.Machines.FirstOrDefaultAsync(m => m.Id == request.MachineId, ct);
        if (machine == null)
        {
            return Result<MachineRealtimeStatusDto>.Failure($"Machine #{request.MachineId} not found.");
        }

        var newStatus = (request.NewStatus ?? "IDLE").Trim().ToUpperInvariant();
        var validStatuses = new[] { "RUNNING", "IDLE", "DOWNTIME", "CHANGEOVER", "MAINTENANCE", "OFFLINE" };
        if (!validStatuses.Contains(newStatus))
        {
            return Result<MachineRealtimeStatusDto>.Failure($"Invalid status '{newStatus}'. Allowed values: {string.Join(", ", validStatuses)}.");
        }

        MachineReasonCode? reason = null;
        if (request.ReasonId.HasValue && request.ReasonId.Value > 0)
        {
            reason = await _db.MachineReasonCodes.FirstOrDefaultAsync(r => r.Id == request.ReasonId.Value, ct);
        }

        var now = DateTime.UtcNow;

        // Close currently active open history record
        var openRecord = await _db.MachineStatusHistories
            .FirstOrDefaultAsync(h => h.MachineId == machine.Id && h.EndTime == null, ct);

        if (openRecord != null)
        {
            openRecord.EndTime = now;
            openRecord.DurationSeconds = (long)Math.Max(0, (now - openRecord.StartTime).TotalSeconds);
            openRecord.UpdatedDate = now;
        }

        // Create new history event
        var newRecord = new MachineStatusHistory
        {
            MachineId = machine.Id,
            Status = newStatus,
            StartTime = now,
            EndTime = null,
            ReasonId = reason?.Id,
            ReasonCode = reason?.ReasonCode,
            ReasonName = reason?.ReasonName,
            OperatorName = request.OperatorName?.Trim(),
            WorkOrderCode = request.WorkOrderCode?.Trim(),
            Remarks = request.Remarks?.Trim(),
            CreatedDate = now
        };

        _db.MachineStatusHistories.Add(newRecord);

        // Synchronize legacy Machine.Status (1: Running, 0: Stopped/Other)
        machine.Status = newStatus == "RUNNING" ? 1 : 0;
        machine.UpdatedDate = now;

        await _db.SaveChangesAsync(ct);

        var updated = await GetMachineStatusAsync(machine.Id, ct);
        return Result<MachineRealtimeStatusDto>.Success(updated!);
    }

    #endregion

    #region History

    public async Task<List<MachineStatusHistoryDto>> GetStatusHistoryAsync(
        ulong? machineId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default)
    {
        var query = _db.MachineStatusHistories.AsNoTracking();

        if (machineId.HasValue && machineId.Value > 0)
        {
            query = query.Where(h => h.MachineId == machineId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(h => h.StartTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(h => h.StartTime <= toDate.Value);
        }

        var list = await query.OrderByDescending(h => h.StartTime).Take(500).ToListAsync(ct);
        if (!list.Any()) return new List<MachineStatusHistoryDto>();

        var mIds = list.Select(h => h.MachineId).Distinct().ToList();
        var machines = await _db.Machines.AsNoTracking()
            .Where(m => mIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, m => new { m.MachineCode, m.MachineName }, ct);

        var now = DateTime.UtcNow;

        return list.Select(h =>
        {
            machines.TryGetValue(h.MachineId, out var mInfo);
            var durSec = h.EndTime.HasValue ? (h.DurationSeconds ?? 0) : (long)(now - h.StartTime).TotalSeconds;

            return new MachineStatusHistoryDto
            {
                Id = h.Id,
                MachineId = h.MachineId,
                MachineCode = mInfo?.MachineCode ?? string.Empty,
                MachineName = mInfo?.MachineName ?? string.Empty,
                Status = h.Status,
                StartTime = h.StartTime,
                EndTime = h.EndTime,
                DurationSeconds = durSec,
                DurationMinutes = Math.Round(durSec / 60.0, 1),
                ReasonId = h.ReasonId,
                ReasonCode = h.ReasonCode,
                ReasonName = h.ReasonName,
                OperatorName = h.OperatorName,
                WorkOrderCode = h.WorkOrderCode,
                Remarks = h.Remarks
            };
        }).ToList();
    }

    #endregion

    #region Defect Logging (Scrap / Rework)

    public async Task<Result<MachineDefectLogDto>> LogDefectAsync(LogMachineDefectRequest request, CancellationToken ct = default)
    {
        var machine = await _db.Machines.FirstOrDefaultAsync(m => m.Id == request.MachineId, ct);
        if (machine == null)
        {
            return Result<MachineDefectLogDto>.Failure($"Machine #{request.MachineId} not found.");
        }

        var defectType = (request.DefectType ?? "SCRAP").Trim().ToUpperInvariant();
        if (defectType != "SCRAP" && defectType != "REWORK")
        {
            return Result<MachineDefectLogDto>.Failure("Defect type must be 'SCRAP' or 'REWORK'.");
        }

        if (request.Quantity <= 0)
        {
            return Result<MachineDefectLogDto>.Failure("Defect quantity must be greater than 0.");
        }

        MachineReasonCode? reason = null;
        if (request.ReasonId.HasValue && request.ReasonId.Value > 0)
        {
            reason = await _db.MachineReasonCodes.FirstOrDefaultAsync(r => r.Id == request.ReasonId.Value, ct);
        }

        string? productCode = request.ProductCode?.Trim();
        if (request.ProductId.HasValue && request.ProductId.Value > 0 && string.IsNullOrWhiteSpace(productCode))
        {
            var prod = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId.Value, ct);
            productCode = prod?.ProductCode;
        }

        var now = DateTime.UtcNow;
        var entity = new MachineDefectLog
        {
            MachineId = machine.Id,
            DefectType = defectType,
            ReasonId = reason?.Id,
            ReasonCode = reason?.ReasonCode,
            ReasonName = reason?.ReasonName,
            Quantity = request.Quantity,
            ProductId = request.ProductId,
            ProductCode = productCode,
            WorkOrderCode = request.WorkOrderCode?.Trim(),
            LoggedTime = now,
            OperatorName = request.OperatorName?.Trim(),
            Remarks = request.Remarks?.Trim(),
            CreatedDate = now
        };

        _db.MachineDefectLogs.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result<MachineDefectLogDto>.Success(new MachineDefectLogDto
        {
            Id = entity.Id,
            MachineId = entity.MachineId,
            MachineCode = machine.MachineCode,
            MachineName = machine.MachineName,
            DefectType = entity.DefectType,
            ReasonId = entity.ReasonId,
            ReasonCode = entity.ReasonCode,
            ReasonName = entity.ReasonName,
            Quantity = entity.Quantity,
            ProductId = entity.ProductId,
            ProductCode = entity.ProductCode,
            WorkOrderCode = entity.WorkOrderCode,
            LoggedTime = entity.LoggedTime,
            OperatorName = entity.OperatorName,
            Remarks = entity.Remarks
        });
    }

    public async Task<List<MachineDefectLogDto>> GetDefectLogsAsync(
        ulong? machineId = null, string? defectType = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default)
    {
        var query = _db.MachineDefectLogs.AsNoTracking();

        if (machineId.HasValue && machineId.Value > 0)
        {
            query = query.Where(d => d.MachineId == machineId.Value);
        }

        if (!string.IsNullOrWhiteSpace(defectType))
        {
            var dt = defectType.Trim().ToUpperInvariant();
            query = query.Where(d => d.DefectType == dt);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(d => d.LoggedTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(d => d.LoggedTime <= toDate.Value);
        }

        var list = await query.OrderByDescending(d => d.LoggedTime).Take(500).ToListAsync(ct);
        if (!list.Any()) return new List<MachineDefectLogDto>();

        var mIds = list.Select(d => d.MachineId).Distinct().ToList();
        var machines = await _db.Machines.AsNoTracking()
            .Where(m => mIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, m => new { m.MachineCode, m.MachineName }, ct);

        return list.Select(d =>
        {
            machines.TryGetValue(d.MachineId, out var mInfo);
            return new MachineDefectLogDto
            {
                Id = d.Id,
                MachineId = d.MachineId,
                MachineCode = mInfo?.MachineCode ?? string.Empty,
                MachineName = mInfo?.MachineName ?? string.Empty,
                DefectType = d.DefectType,
                ReasonId = d.ReasonId,
                ReasonCode = d.ReasonCode,
                ReasonName = d.ReasonName,
                Quantity = d.Quantity,
                ProductId = d.ProductId,
                ProductCode = d.ProductCode,
                WorkOrderCode = d.WorkOrderCode,
                LoggedTime = d.LoggedTime,
                OperatorName = d.OperatorName,
                Remarks = d.Remarks
            };
        }).ToList();
    }

    #endregion

    private static MachineReasonCodeDto MapReasonDto(MachineReasonCode r) => new()
    {
        Id = r.Id,
        ReasonCode = r.ReasonCode,
        ReasonName = r.ReasonName,
        ReasonType = r.ReasonType,
        Category = r.Category,
        IsPlanned = r.IsPlanned,
        IsActive = r.IsActive ?? false,
        Description = r.Description
    };
}
