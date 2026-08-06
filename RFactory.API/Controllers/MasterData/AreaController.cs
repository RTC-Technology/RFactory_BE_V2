using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for the Area master data.
/// </summary>
[ApiController]
[Route("api/master-data/areas")]
[Authorize]
public class AreaController : ControllerBase
{
    private readonly IAreaService _areaService;

    public AreaController(IAreaService areaService)
    {
        _areaService = areaService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Area.View)]
    public async Task<ActionResult<ApiResponse<List<AreaDto>>>> GetAll(CancellationToken ct)
    {
        var areas = await _areaService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(areas));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Area.View)]
    public async Task<ActionResult<ApiResponse<AreaDto>>> GetById(ulong id, CancellationToken ct)
    {
        var area = await _areaService.GetByIdAsync(id, ct);
        if (area is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Area {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(area));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Area.Add)]
    public async Task<ActionResult<ApiResponse<AreaDto>>> Create(CreateAreaRequest request, CancellationToken ct)
    {
        var result = await _areaService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Area.Edit)]
    public async Task<ActionResult<ApiResponse<AreaDto>>> Update(ulong id, UpdateAreaRequest request, CancellationToken ct)
    {
        var result = await _areaService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Area.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _areaService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Area deleted."));
    }
}
