using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers.Administration;

/// <summary>
/// CRUD endpoints for navigation menu items.
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
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetAll(CancellationToken ct)
    {
        var menus = await _menuService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(menus));
    }

    [HttpGet("{id:long}")]
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
