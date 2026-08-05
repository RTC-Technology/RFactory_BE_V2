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
/// Payload carrying the refresh token used to obtain a new token pair.
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Token pair returned to the client after a successful login or refresh.
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Profile of the currently authenticated user, returned by GET /api/auth/me.
/// Permissions is a list of FunctionCode values; currently empty for non-admin
/// users since UserGroup-based right distribution is not wired up yet.
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
