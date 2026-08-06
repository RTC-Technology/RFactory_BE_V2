using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product;

/// <summary>
/// CRUD endpoints for BOM.
/// </summary>
[ApiController]
[Route("api/product/boms")]
[Authorize]
public class BomController : ControllerBase
{
    private readonly IBomService _bomService;

    public BomController(IBomService service)
    {
        _bomService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Bom.View)]
    public async Task<ActionResult<ApiResponse<List<BomDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _bomService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Bom.View)]
    public async Task<ActionResult<ApiResponse<BomDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _bomService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"BOM {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Bom.Add)]
    public async Task<ActionResult<ApiResponse<BomDto>>> Create(CreateBomRequest request, CancellationToken ct)
    {
        var result = await _bomService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Bom.Edit)]
    public async Task<ActionResult<ApiResponse<BomDto>>> Update(ulong id, UpdateBomRequest request, CancellationToken ct)
    {
        var result = await _bomService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Bom.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _bomService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "BOM deleted."));
    }
}