using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product;

/// <summary>
/// CRUD endpoints for Routing operation (công đoạn).
/// </summary>
[ApiController]
[Route("api/product/routing-operations")]
[Authorize]
public class RoutingOperationController : ControllerBase
{
    private readonly IRoutingOperationService _routingOperationService;

    public RoutingOperationController(IRoutingOperationService service)
    {
        _routingOperationService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.RoutingOperation.View)]
    public async Task<ActionResult<ApiResponse<List<RoutingOperationDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _routingOperationService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.RoutingOperation.View)]
    public async Task<ActionResult<ApiResponse<RoutingOperationDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _routingOperationService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Routing operation {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.RoutingOperation.Add)]
    public async Task<ActionResult<ApiResponse<RoutingOperationDto>>> Create(CreateRoutingOperationRequest request, CancellationToken ct)
    {
        var result = await _routingOperationService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.RoutingOperation.Edit)]
    public async Task<ActionResult<ApiResponse<RoutingOperationDto>>> Update(ulong id, UpdateRoutingOperationRequest request, CancellationToken ct)
    {
        var result = await _routingOperationService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.RoutingOperation.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _routingOperationService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Routing operation deleted."));
    }
}
