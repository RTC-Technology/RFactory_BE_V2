namespace RFactory.Application.Modules.Auth.DTOs;

/// <summary>
/// Credentials submitted to obtain a new access/refresh token pair.
/// </summary>
public class LoginRequest
{
    public string LoginName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Carries the refresh token between the API layer and the service.
///
/// Not a request body any more: the controller reads the token out of the httpOnly
/// cookie, so it never travels through JavaScript.
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Internal result of a login or refresh — both tokens. The refresh token stops here:
/// the controller moves it into an httpOnly cookie and returns
/// <see cref="AuthTokenResponse"/> to the client instead.
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// What the client actually receives. Deliberately has no refresh token — putting one in
/// the body would hand it straight back to any script running on the page, which is the
/// whole thing the cookie exists to prevent.
/// </summary>
public class AuthTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Profile of the currently authenticated user, returned by GET /api/auth/me.
/// Permissions holds the FunctionCode values the user's groups grant. Admins are not
/// folded in — IsAdmin is a bypass applied at the point of the check, so "granted" and
/// "bypassed" stay distinguishable.
/// </summary>
public class UserProfileDto
{
    public ulong Id { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public List<string> Permissions { get; set; } = new();
}
