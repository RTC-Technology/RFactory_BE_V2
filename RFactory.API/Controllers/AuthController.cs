using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Application.Modules.Auth.DTOs;
using RFactory.Application.Modules.Auth.Services;
using RFactory.Shared.Abstractions;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers;

/// <summary>
/// Login, refresh-token rotation, logout, profile, and user-scoped menu endpoints.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMenuService _menuService;
    private readonly IUser _currentUser;

    public AuthController(IAuthService authService, IMenuService menuService, IUser currentUser)
    {
        _authService = authService;
        _menuService = menuService;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.Unauthorized));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(request, ct);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.Unauthorized));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object?>>> Logout(CancellationToken ct)
    {
        if (!ulong.TryParse(_currentUser.Id, out var userId))
        {
            return Unauthorized(ApiResponseFactory.Fail("Invalid user context.", HttpStatusCode.Unauthorized));
        }

        var result = await _authService.LogoutAsync(userId, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Logged out."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Me(CancellationToken ct)
    {
        if (!ulong.TryParse(_currentUser.Id, out var userId))
        {
            return Unauthorized(ApiResponseFactory.Fail("Invalid user context.", HttpStatusCode.Unauthorized));
        }

        var result = await _authService.GetProfileAsync(userId, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpGet("menus")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetMyMenus(CancellationToken ct)
    {
        if (!ulong.TryParse(_currentUser.Id, out var userId))
        {
            return Unauthorized(ApiResponseFactory.Fail("Invalid user context.", HttpStatusCode.Unauthorized));
        }

        var menus = await _menuService.GetMenusForUserAsync(userId, _currentUser.IsAdmin, ct);
        return Ok(ApiResponseFactory.Success(menus));
    }
}
