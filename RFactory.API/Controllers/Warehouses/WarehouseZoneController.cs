using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Warehouses.DTOs;
using RFactory.Application.Modules.Warehouses.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Warehouses;

/// <summary>
/// CRUD endpoints for the zones a warehouse is divided into.
/// </summary>
[ApiController]
[Route("api/warehouse/zones")]
[Authorize]
public class WarehouseZoneController : ControllerBase
{
    private readonly IWarehouseZoneService _service;

    public WarehouseZoneController(IWarehouseZoneService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.WarehouseZone.View)]
    public async Task<ActionResult<ApiResponse<List<WarehouseZoneDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.WarehouseZone.View)]
    public async Task<ActionResult<ApiResponse<WarehouseZoneDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Warehouse zone {id} was not found.", HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.WarehouseZone.Add)]
    public async Task<ActionResult<ApiResponse<WarehouseZoneDto>>> Create(CreateWarehouseZoneRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.WarehouseZone.Edit)]
    public async Task<ActionResult<ApiResponse<WarehouseZoneDto>>> Update(ulong id, UpdateWarehouseZoneRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.WarehouseZone.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Warehouse zone deleted."));
    }
}
