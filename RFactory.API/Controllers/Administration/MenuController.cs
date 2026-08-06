using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Administration;

/// <summary>
/// CRUD endpoints for navigation menu items.
///
/// Reads are open to any signed-in user because other screens depend on them — the
/// permission screen, for one, reads menus to warn before deleting a permission a menu
/// still points at. Only mutations carry <see cref="RequirePermissionAttribute"/>.
/// </summary>
[ApiController]
[Route("api/administration/menus")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Menu.View)]
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetAll(CancellationToken ct)
    {
        var menus = await _menuService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(menus));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Menu.View)]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetById(ulong id, CancellationToken ct)
    {
        var menu = await _menuService.GetByIdAsync(id, ct);
        if (menu is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Menu {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(menu));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Menu.Add)]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create(CreateMenuRequest request, CancellationToken ct)
    {
        var result = await _menuService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Menu.Edit)]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Update(ulong id, UpdateMenuRequest request, CancellationToken ct)
    {
        var result = await _menuService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Menu.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _menuService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Menu deleted."));
    }
}
