using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for unit conversions.
/// </summary>
[ApiController]
[Route("api/master-data/unit-conversions")]
[Authorize]
public class UnitConversionController : ControllerBase
{
    private readonly IUnitConversionService _unitConversionService;

    public UnitConversionController(IUnitConversionService service)
    {
        _unitConversionService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.UnitConversion.View)]
    public async Task<ActionResult<ApiResponse<List<UnitConversionDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _unitConversionService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.UnitConversion.View)]
    public async Task<ActionResult<ApiResponse<UnitConversionDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _unitConversionService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Unit conversion {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.UnitConversion.Add)]
    public async Task<ActionResult<ApiResponse<UnitConversionDto>>> Create(CreateUnitConversionRequest request, CancellationToken ct)
    {
        var result = await _unitConversionService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.UnitConversion.Edit)]
    public async Task<ActionResult<ApiResponse<UnitConversionDto>>> Update(ulong id, UpdateUnitConversionRequest request, CancellationToken ct)
    {
        var result = await _unitConversionService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.UnitConversion.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _unitConversionService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Unit conversion deleted."));
    }
}