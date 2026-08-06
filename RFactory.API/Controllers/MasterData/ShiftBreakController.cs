using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for the breaks inside a shift.
/// </summary>
[ApiController]
[Route("api/master-data/shift-breaks")]
[Authorize]
public class ShiftBreakController : ControllerBase
{
    private readonly IShiftBreakService _shiftBreakService;

    public ShiftBreakController(IShiftBreakService shiftBreakService)
    {
        _shiftBreakService = shiftBreakService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.ShiftBreak.View)]
    public async Task<ActionResult<ApiResponse<List<ShiftBreakDto>>>> GetAll(CancellationToken ct)
    {
        var breaks = await _shiftBreakService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(breaks));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.ShiftBreak.View)]
    public async Task<ActionResult<ApiResponse<ShiftBreakDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _shiftBreakService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Shift break {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.ShiftBreak.Add)]
    public async Task<ActionResult<ApiResponse<ShiftBreakDto>>> Create(CreateShiftBreakRequest request, CancellationToken ct)
    {
        var result = await _shiftBreakService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.ShiftBreak.Edit)]
    public async Task<ActionResult<ApiResponse<ShiftBreakDto>>> Update(ulong id, UpdateShiftBreakRequest request, CancellationToken ct)
    {
        var result = await _shiftBreakService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.ShiftBreak.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _shiftBreakService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Shift break deleted."));
    }
}
