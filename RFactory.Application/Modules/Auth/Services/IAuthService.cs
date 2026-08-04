using RFactory.Application.Modules.Auth.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Auth.Services;

/// <summary>
/// Handles login, refresh-token rotation and logout for the credential-based auth flow.
/// </summary>
public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task<Result> LogoutAsync(ulong userId, CancellationToken ct = default);
}
