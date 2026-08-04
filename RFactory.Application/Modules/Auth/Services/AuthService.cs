using RFactory.Application.Modules.Auth.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using RFactory.Shared.Security;

namespace RFactory.Application.Modules.Auth.Services;

/// <summary>
/// Credential-based login combined with refresh-token rotation. Refresh tokens are
/// stored on the <see cref="User"/> row (single active session per user).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly TimeProvider _clock;

    public AuthService(
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        TimeProvider clock)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _clock = clock;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.FirstOrDefault(u => u.LoginName == request.LoginName, ct);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid login name or password.");
        }

        var response = await IssueTokenPairAsync(user, ct);
        return Result<AuthResponse>.Success(response);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.FirstOrDefault(u => u.RefreshToken == request.RefreshToken, ct);
        var now = _clock.GetUtcNow().UtcDateTime;

        if (user is null || user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= now)
        {
            return Result<AuthResponse>.Failure("Invalid or expired refresh token.");
        }

        var response = await IssueTokenPairAsync(user, ct);
        return Result<AuthResponse>.Success(response);
    }

    public async Task<Result> LogoutAsync(ulong userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetById(userId, ct);
        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userRepository.Update(user, ct);

        return Result.Success();
    }

    private async Task<AuthResponse> IssueTokenPairAsync(User user, CancellationToken ct)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var now = _clock.GetUtcNow().UtcDateTime;

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = now.AddDays(_tokenService.RefreshTokenExpireDays);
        await _userRepository.Update(user, ct);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = now.AddMinutes(_tokenService.AccessTokenExpireMinutes)
        };
    }
}
