using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Administration;

/// <summary>
/// CRUD endpoints for function groups (permission groups).
/// </summary>
[ApiController]
[Route("api/administration/function-groups")]
[Authorize]
public class FunctionGroupController : ControllerBase
{
    private readonly IFunctionGroupService _functionGroupService;

    public FunctionGroupController(IFunctionGroupService functionGroupService)
    {
        _functionGroupService = functionGroupService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.FunctionGroup.View)]
    public async Task<ActionResult<ApiResponse<List<FunctionGroupDto>>>> GetAll(CancellationToken ct)
    {
        var groups = await _functionGroupService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(groups));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.FunctionGroup.View)]
    public async Task<ActionResult<ApiResponse<FunctionGroupDto>>> GetById(ulong id, CancellationToken ct)
    {
        var group = await _functionGroupService.GetByIdAsync(id, ct);
        if (group is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Function group {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(group));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.FunctionGroup.Add)]
    //[RequirePermission(PermissionCodes.FunctionView)]
    public async Task<ActionResult<ApiResponse<FunctionGroupDto>>> Create(CreateFunctionGroupRequest request, CancellationToken ct)
    {
        var result = await _functionGroupService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.FunctionGroup.Edit)]
    //[RequirePermission(PermissionCodes.FunctionManage)]
    public async Task<ActionResult<ApiResponse<FunctionGroupDto>>> Update(ulong id, UpdateFunctionGroupRequest request, CancellationToken ct)
    {
        var result = await _functionGroupService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.FunctionGroup.Delete)]
    //[RequirePermission(PermissionCodes.FunctionManage)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _functionGroupService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Function group deleted."));
    }
}
