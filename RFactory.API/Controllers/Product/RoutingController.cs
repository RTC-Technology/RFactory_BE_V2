using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product;

/// <summary>
/// CRUD endpoints for Routing (process specification).
/// </summary>
[ApiController]
[Route("api/product/routings")]
[Authorize]
public class RoutingController : ControllerBase
{
    private readonly IRoutingService _routingService;

    public RoutingController(IRoutingService service)
    {
        _routingService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Routing.View)]
    public async Task<ActionResult<ApiResponse<List<RoutingDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _routingService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Routing.View)]
    public async Task<ActionResult<ApiResponse<RoutingDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _routingService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Routing {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Routing.Add)]
    public async Task<ActionResult<ApiResponse<RoutingDto>>> Create(CreateRoutingRequest request, CancellationToken ct)
    {
        var result = await _routingService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Routing.Edit)]
    public async Task<ActionResult<ApiResponse<RoutingDto>>> Update(ulong id, UpdateRoutingRequest request, CancellationToken ct)
    {
        var result = await _routingService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Routing.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _routingService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Routing deleted."));
    }
}
