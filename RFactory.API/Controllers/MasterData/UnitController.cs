using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for units of measure.
/// </summary>
[ApiController]
[Route("api/master-data/units")]
[Authorize]
public class UnitController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Unit.View)]
    public async Task<ActionResult<ApiResponse<List<UnitDto>>>> GetAll(CancellationToken ct)
    {
        var units = await _unitService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(units));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Unit.View)]
    public async Task<ActionResult<ApiResponse<UnitDto>>> GetById(ulong id, CancellationToken ct)
    {
        var unit = await _unitService.GetByIdAsync(id, ct);
        if (unit is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Unit {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(unit));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Unit.Add)]
    public async Task<ActionResult<ApiResponse<UnitDto>>> Create(CreateUnitRequest request, CancellationToken ct)
    {
        var result = await _unitService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Unit.Edit)]
    public async Task<ActionResult<ApiResponse<UnitDto>>> Update(ulong id, UpdateUnitRequest request, CancellationToken ct)
    {
        var result = await _unitService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Unit.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _unitService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Unit deleted."));
    }
}
