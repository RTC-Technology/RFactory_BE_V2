using RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Auth.Services;

/// <summary>
/// Generates access and refresh tokens for authenticated users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a signed JWT access token carrying the user's identity claims.
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Creates a cryptographically random refresh token string.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Access token lifetime, used by callers to report expiry to clients.
    /// </summary>
    int AccessTokenExpireMinutes { get; }

    /// <summary>
    /// Refresh token lifetime in days.
    /// </summary>
    int RefreshTokenExpireDays { get; }
}
