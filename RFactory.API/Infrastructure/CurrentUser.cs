using System.Security.Claims;
using RFactory.Shared.Abstractions;

namespace RFactory.API.Infrastructure;

/// <summary>
/// Resolves the current authenticated user from <see cref="IHttpContextAccessor"/>.
/// Registered as Scoped so it captures the per-request HttpContext.
/// </summary>
public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string? Id => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}
