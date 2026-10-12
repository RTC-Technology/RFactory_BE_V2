using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Application.Modules.Equipment.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Equipment;

[ApiController]
[Route("api/equipment/machine-status")]
[Authorize]
public class MachineStatusController : ControllerBase
{
    private readonly IMachineStatusService _statusService;

    public MachineStatusController(IMachineStatusService statusService)
    {
        _statusService = statusService;
    }

    // ── Real-time Status Monitoring ─────────────────────────────

    [HttpGet("realtime")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<List<MachineRealtimeStatusDto>>>> GetRealtimeStatuses([FromQuery] long? lineId, CancellationToken ct)
    {
        var data = await _statusService.GetRealtimeStatusesAsync(lineId, ct);
        return Ok(ApiResponseFactory.Success(data));
    }

    [HttpGet("summary")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<MachineStatusDashboardSummaryDto>>> GetSummary(CancellationToken ct)
    {
        var data = await _statusService.GetDashboardSummaryAsync(ct);
        return Ok(ApiResponseFactory.Success(data));
    }

    [HttpGet("{machineId:long}")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<MachineRealtimeStatusDto>>> GetMachineStatus(ulong machineId, CancellationToken ct)
    {
        var data = await _statusService.GetMachineStatusAsync(machineId, ct);
        if (data == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Machine #{machineId} not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(data));
    }

    [HttpPost("change")]
    [RequirePermission(PermissionCodes.MachineStatus.Edit)]
    public async Task<ActionResult<ApiResponse<MachineRealtimeStatusDto>>> ChangeStatus([FromBody] ChangeMachineStatusRequest request, CancellationToken ct)
    {
        var result = await _statusService.ChangeStatusAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    // ── Status History & Downtime Log ───────────────────────────

    [HttpGet("history")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<List<MachineStatusHistoryDto>>>> GetHistory(
        [FromQuery] ulong? machineId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken ct)
    {
        var data = await _statusService.GetStatusHistoryAsync(machineId, fromDate, toDate, ct);
        return Ok(ApiResponseFactory.Success(data));
    }

    // ── Defect Logging (Scrap / Rework) ─────────────────────────

    [HttpPost("defect")]
    [RequirePermission(PermissionCodes.MachineStatus.Add)]
    public async Task<ActionResult<ApiResponse<MachineDefectLogDto>>> LogDefect([FromBody] LogMachineDefectRequest request, CancellationToken ct)
    {
        var result = await _statusService.LogDefectAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpGet("defect-logs")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<List<MachineDefectLogDto>>>> GetDefectLogs(
        [FromQuery] ulong? machineId, [FromQuery] string? defectType, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken ct)
    {
        var data = await _statusService.GetDefectLogsAsync(machineId, defectType, fromDate, toDate, ct);
        return Ok(ApiResponseFactory.Success(data));
    }

    // ── Reason Codes Management ─────────────────────────────────

    [HttpGet("reasons")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<List<MachineReasonCodeDto>>>> GetReasons([FromQuery] string? type, CancellationToken ct)
    {
        var data = await _statusService.GetAllReasonsAsync(type, ct);
        return Ok(ApiResponseFactory.Success(data));
    }

    [HttpGet("reasons/{id:long}")]
    [RequirePermission(PermissionCodes.MachineStatus.View)]
    public async Task<ActionResult<ApiResponse<MachineReasonCodeDto>>> GetReasonById(ulong id, CancellationToken ct)
    {
        var data = await _statusService.GetReasonByIdAsync(id, ct);
        if (data == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Reason #{id} not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(data));
    }

    [HttpPost("reasons")]
    [RequirePermission(PermissionCodes.MachineStatus.Add)]
    public async Task<ActionResult<ApiResponse<MachineReasonCodeDto>>> CreateReason([FromBody] MachineReasonCodeRequest request, CancellationToken ct)
    {
        var result = await _statusService.CreateReasonAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("reasons/{id:long}")]
    [RequirePermission(PermissionCodes.MachineStatus.Edit)]
    public async Task<ActionResult<ApiResponse<MachineReasonCodeDto>>> UpdateReason(ulong id, [FromBody] MachineReasonCodeRequest request, CancellationToken ct)
    {
        var result = await _statusService.UpdateReasonAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("reasons/{id:long}")]
    [RequirePermission(PermissionCodes.MachineStatus.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteReason(ulong id, CancellationToken ct)
    {
        var result = await _statusService.DeleteReasonAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }
}
