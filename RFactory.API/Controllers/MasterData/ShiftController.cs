using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for working shifts.
/// </summary>
[ApiController]
[Route("api/master-data/shifts")]
[Authorize]
public class ShiftController : ControllerBase
{
    private readonly IShiftService _shiftService;

    public ShiftController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Shift.View)]
    public async Task<ActionResult<ApiResponse<List<ShiftDto>>>> GetAll(CancellationToken ct)
    {
        var shifts = await _shiftService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(shifts));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Shift.View)]
    public async Task<ActionResult<ApiResponse<ShiftDto>>> GetById(ulong id, CancellationToken ct)
    {
        var shift = await _shiftService.GetByIdAsync(id, ct);
        if (shift is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Shift {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(shift));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Shift.Add)]
    public async Task<ActionResult<ApiResponse<ShiftDto>>> Create(CreateShiftRequest request, CancellationToken ct)
    {
        var result = await _shiftService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Shift.Edit)]
    public async Task<ActionResult<ApiResponse<ShiftDto>>> Update(ulong id, UpdateShiftRequest request, CancellationToken ct)
    {
        var result = await _shiftService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Shift.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _shiftService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Shift deleted."));
    }
}
