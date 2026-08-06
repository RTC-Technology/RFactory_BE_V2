using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Application.Modules.Auth.DTOs;
using RFactory.Application.Modules.Auth.Services;
using RFactory.Shared.Abstractions;
using RFactory.Shared.Api;
using RFactory.Shared.Security;

namespace RFactory.API.Controllers;

/// <summary>
/// Login, refresh-token rotation, logout, profile, and user-scoped menu endpoints.
///
/// The refresh token lives in an httpOnly cookie and is never part of a response body.
/// That is the one thing script running on the page cannot read, so an XSS foothold can
/// at worst use the short-lived access token — it cannot walk away with the ability to
/// mint new ones.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Scoped to /api/auth so the browser does not attach it to every other API call —
    /// the only endpoints that need it are refresh and logout.
    /// </summary>
    private const string RefreshCookieName = "rf_refresh_token";
    private const string RefreshCookiePath = "/api/auth";

    private readonly IAuthService _authService;
    private readonly IMenuService _menuService;
    private readonly IUser _currentUser;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        IAuthService authService,
        IMenuService menuService,
        IUser currentUser,
        IOptions<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _menuService = menuService;
        _currentUser = currentUser;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.Unauthorized));
        }

        return Ok(ApiResponseFactory.Success(IssueTokens(result.Data!)));
    }

    /// <summary>
    /// Takes no body: the refresh token comes from the cookie the browser attaches, which
    /// is also what stops a page script from replaying somebody else's token.
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> RefreshToken(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(ApiResponseFactory.Fail("No refresh token was supplied.", HttpStatusCode.Unauthorized));
        }

        var result = await _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = refreshToken }, ct);
        if (!result.Succeeded)
        {
            // The cookie is stale or forged; clear it so the browser stops resending it.
            ClearRefreshTokenCookie();
            return Unauthorized(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.Unauthorized));
        }

        return Ok(ApiResponseFactory.Success(IssueTokens(result.Data!)));
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

        // Cleared regardless of the outcome: leaving a cookie behind that the server has
        // already invalidated only produces confusing 401s on the next visit.
        ClearRefreshTokenCookie();

        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Logged out."));
    }

    // ─── Refresh cookie ──────────────────────────────────────────────────────────

    /// <summary>Moves the refresh token into the cookie and returns only what the client
    /// is allowed to hold.</summary>
    private AuthTokenResponse IssueTokens(AuthResponse tokens)
    {
        Response.Cookies.Append(RefreshCookieName, tokens.RefreshToken, BuildCookieOptions(
            DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenExpireDays)));

        return new AuthTokenResponse
        {
            AccessToken = tokens.AccessToken,
            ExpiresAt = tokens.ExpiresAt,
        };
    }

    private void ClearRefreshTokenCookie()
    {
        // Same options as when it was written — a delete whose attributes differ from the
        // original does not match the cookie and silently leaves it in place.
        Response.Cookies.Append(RefreshCookieName, string.Empty, BuildCookieOptions(DateTimeOffset.UnixEpoch));
    }

    private static CookieOptions BuildCookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        // SameSite=None is what lets the SPA on its own origin send the cookie to the API,
        // and browsers only accept that pairing over HTTPS.
        Secure = true,
        SameSite = SameSiteMode.None,
        Path = RefreshCookiePath,
        Expires = expires,
    };

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
