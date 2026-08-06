using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for unit categorys.
/// </summary>
[ApiController]
[Route("api/master-data/unit-categories")]
[Authorize]
public class UnitCategoryController : ControllerBase
{
    private readonly IUnitCategoryService _unitCategoryService;

    public UnitCategoryController(IUnitCategoryService service)
    {
        _unitCategoryService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.UnitCategory.View)]
    public async Task<ActionResult<ApiResponse<List<UnitCategoryDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _unitCategoryService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.UnitCategory.View)]
    public async Task<ActionResult<ApiResponse<UnitCategoryDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _unitCategoryService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Unit category {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.UnitCategory.Add)]
    public async Task<ActionResult<ApiResponse<UnitCategoryDto>>> Create(CreateUnitCategoryRequest request, CancellationToken ct)
    {
        var result = await _unitCategoryService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.UnitCategory.Edit)]
    public async Task<ActionResult<ApiResponse<UnitCategoryDto>>> Update(ulong id, UpdateUnitCategoryRequest request, CancellationToken ct)
    {
        var result = await _unitCategoryService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.UnitCategory.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _unitCategoryService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Unit category deleted."));
    }
}