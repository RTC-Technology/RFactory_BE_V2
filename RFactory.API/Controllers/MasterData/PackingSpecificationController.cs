using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// Endpoints for managing packaging specifications across products and logistics tiers.
/// </summary>
[ApiController]
[Route("api/packing-specifications")]
[Authorize]
public class PackingSpecificationController : ControllerBase
{
    private readonly IPackingSpecificationService _service;

    public PackingSpecificationController(IPackingSpecificationService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.PackingSpecification.View)]
    public async Task<ActionResult<ApiResponse<List<PackingSpecificationDto>>>> GetAll(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.PackingSpecification.View)]
    public async Task<ActionResult<ApiResponse<PackingSpecificationDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Packing specification #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpGet("product/{productId:long}")]
    [RequirePermission(PermissionCodes.PackingSpecification.View)]
    public async Task<ActionResult<ApiResponse<List<PackingSpecificationDto>>>> GetByProductId(ulong productId, CancellationToken ct)
    {
        var list = await _service.GetByProductIdAsync(productId, ct);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("product/{productId:long}/default")]
    [RequirePermission(PermissionCodes.PackingSpecification.View)]
    public async Task<ActionResult<ApiResponse<PackingSpecificationDto>>> GetDefaultByProductId(ulong productId, CancellationToken ct)
    {
        var item = await _service.GetDefaultByProductIdAsync(productId, ct);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Fail($"No packing specification found for product #{productId}", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.PackingSpecification.Add)]
    public async Task<ActionResult<ApiResponse<PackingSpecificationDto>>> Create([FromBody] PackingSpecificationRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.PackingSpecification.Edit)]
    public async Task<ActionResult<ApiResponse<PackingSpecificationDto>>> Update(ulong id, [FromBody] PackingSpecificationRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.PackingSpecification.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(true));
    }

    [HttpPost("{id:long}/set-default")]
    [RequirePermission(PermissionCodes.PackingSpecification.Edit)]
    public async Task<ActionResult<ApiResponse<bool>>> SetDefault(ulong id, CancellationToken ct)
    {
        var result = await _service.SetDefaultAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(true));
    }
}
