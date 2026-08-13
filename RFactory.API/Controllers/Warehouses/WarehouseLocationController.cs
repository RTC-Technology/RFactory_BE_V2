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
/// CRUD endpoints for the storage locations inside a warehouse zone.
/// </summary>
[ApiController]
[Route("api/warehouse/locations")]
[Authorize]
public class WarehouseLocationController : ControllerBase
{
    private readonly IWarehouseLocationService _service;

    public WarehouseLocationController(IWarehouseLocationService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.WarehouseLocation.View)]
    public async Task<ActionResult<ApiResponse<List<WarehouseLocationDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.WarehouseLocation.View)]
    public async Task<ActionResult<ApiResponse<WarehouseLocationDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Warehouse location {id} was not found.", HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.WarehouseLocation.Add)]
    public async Task<ActionResult<ApiResponse<WarehouseLocationDto>>> Create(CreateWarehouseLocationRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.WarehouseLocation.Edit)]
    public async Task<ActionResult<ApiResponse<WarehouseLocationDto>>> Update(ulong id, UpdateWarehouseLocationRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.WarehouseLocation.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Warehouse location deleted."));
    }
}
