using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Equipment.Services;

public interface IMachineStatusService
{
    // Reason Codes
    Task<List<MachineReasonCodeDto>> GetAllReasonsAsync(string? reasonType = null, CancellationToken ct = default);
    Task<MachineReasonCodeDto?> GetReasonByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<MachineReasonCodeDto>> CreateReasonAsync(MachineReasonCodeRequest request, CancellationToken ct = default);
    Task<Result<MachineReasonCodeDto>> UpdateReasonAsync(ulong id, MachineReasonCodeRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteReasonAsync(ulong id, CancellationToken ct = default);

    // Real-time Status Monitoring & Transitions
    Task<List<MachineRealtimeStatusDto>> GetRealtimeStatusesAsync(long? lineId = null, CancellationToken ct = default);
    Task<MachineRealtimeStatusDto?> GetMachineStatusAsync(ulong machineId, CancellationToken ct = default);
    Task<MachineStatusDashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken ct = default);
    Task<Result<MachineRealtimeStatusDto>> ChangeStatusAsync(ChangeMachineStatusRequest request, CancellationToken ct = default);

    // History
    Task<List<MachineStatusHistoryDto>> GetStatusHistoryAsync(ulong? machineId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    // Scrap & Rework Logging
    Task<Result<MachineDefectLogDto>> LogDefectAsync(LogMachineDefectRequest request, CancellationToken ct = default);
    Task<List<MachineDefectLogDto>> GetDefectLogsAsync(ulong? machineId = null, string? defectType = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
}
