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
