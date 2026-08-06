using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product;

/// <summary>
/// CRUD endpoints for BOM line.
/// </summary>
[ApiController]
[Route("api/product/bom-details")]
[Authorize]
public class BomDetailController : ControllerBase
{
    private readonly IBomDetailService _bomDetailService;

    public BomDetailController(IBomDetailService service)
    {
        _bomDetailService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.BomDetail.View)]
    public async Task<ActionResult<ApiResponse<List<BomDetailDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _bomDetailService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.BomDetail.View)]
    public async Task<ActionResult<ApiResponse<BomDetailDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _bomDetailService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"BOM line {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.BomDetail.Add)]
    public async Task<ActionResult<ApiResponse<BomDetailDto>>> Create(CreateBomDetailRequest request, CancellationToken ct)
    {
        var result = await _bomDetailService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.BomDetail.Edit)]
    public async Task<ActionResult<ApiResponse<BomDetailDto>>> Update(ulong id, UpdateBomDetailRequest request, CancellationToken ct)
    {
        var result = await _bomDetailService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.BomDetail.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _bomDetailService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "BOM line deleted."));
    }
}