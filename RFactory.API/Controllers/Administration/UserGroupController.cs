using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Administration;

/// <summary>
/// CRUD endpoints for user groups, plus the two assignment sets they own:
/// the functions a group grants and the users that belong to it.
/// </summary>
[ApiController]
[Route("api/administration/user-groups")]
[Authorize]
public class UserGroupController : ControllerBase
{
    private readonly IUserGroupService _userGroupService;

    public UserGroupController(IUserGroupService userGroupService)
    {
        _userGroupService = userGroupService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.UserGroup.View)]
    public async Task<ActionResult<ApiResponse<List<UserGroupDto>>>> GetAll(CancellationToken ct)
    {
        var groups = await _userGroupService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(groups));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.UserGroup.View)]
    public async Task<ActionResult<ApiResponse<UserGroupDto>>> GetById(ulong id, CancellationToken ct)
    {
        var group = await _userGroupService.GetByIdAsync(id, ct);
        if (group is null)
        {
            return NotFound(ApiResponseFactory.Fail($"User group {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(group));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.UserGroup.Add)]
    public async Task<ActionResult<ApiResponse<UserGroupDto>>> Create(CreateUserGroupRequest request, CancellationToken ct)
    {
        var result = await _userGroupService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.UserGroup.Edit)]
    public async Task<ActionResult<ApiResponse<UserGroupDto>>> Update(ulong id, UpdateUserGroupRequest request, CancellationToken ct)
    {
        var result = await _userGroupService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.UserGroup.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _userGroupService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "User group deleted."));
    }

    // ─── Assignments ─────────────────────────────────────────────────────────────

    [HttpGet("{id:long}/functions")]
    [RequirePermission(PermissionCodes.UserGroup.View)]
    public async Task<ActionResult<ApiResponse<List<long>>>> GetFunctions(ulong id, CancellationToken ct)
    {
        var functionIds = await _userGroupService.GetFunctionIdsAsync(id, ct);
        return Ok(ApiResponseFactory.Success(functionIds));
    }

    /// <summary>Replaces the group's whole function set — PUT, not PATCH, on purpose.</summary>
    [HttpPut("{id:long}/functions")]
    [RequirePermission(PermissionCodes.UserGroup.Edit)]
    public async Task<ActionResult<ApiResponse<object?>>> SetFunctions(
        ulong id, SetUserGroupFunctionsRequest request, CancellationToken ct)
    {
        var result = await _userGroupService.SetFunctionsAsync(id, request.FunctionIds, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Function assignments updated."));
    }

    [HttpGet("{id:long}/users")]
    [RequirePermission(PermissionCodes.UserGroup.View)]
    public async Task<ActionResult<ApiResponse<List<long>>>> GetUsers(ulong id, CancellationToken ct)
    {
        var userIds = await _userGroupService.GetUserIdsAsync(id, ct);
        return Ok(ApiResponseFactory.Success(userIds));
    }

    /// <summary>Replaces the group's whole membership — PUT, not PATCH, on purpose.</summary>
    [HttpPut("{id:long}/users")]
    [RequirePermission(PermissionCodes.UserGroup.Edit)]
    public async Task<ActionResult<ApiResponse<object?>>> SetUsers(
        ulong id, SetUserGroupUsersRequest request, CancellationToken ct)
    {
        var result = await _userGroupService.SetUsersAsync(id, request.UserIds, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Membership updated."));
    }
}
