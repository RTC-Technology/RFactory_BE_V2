using Microsoft.Extensions.Caching.Memory;
using RFactory.Infrastructure.Persistence;

namespace RFactory.Application.Modules.Administration.Services;

public class UserPermissionService : IUserPermissionService
{
    /// <summary>
    /// Safety net only — <see cref="IPermissionCacheSignal"/> is what actually keeps this
    /// honest. Short enough that a grant change missed by the signal still corrects itself
    /// within a minute rather than lasting the process.
    /// </summary>
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromSeconds(60);

    private readonly IUserPermissionQuery _query;
    private readonly IMemoryCache _cache;
    private readonly IPermissionCacheSignal _signal;

    public UserPermissionService(
        IUserPermissionQuery query,
        IMemoryCache cache,
        IPermissionCacheSignal signal)
    {
        _query = query;
        _cache = cache;
        _signal = signal;
    }

    /// <summary>
    /// Cached because <c>RequirePermissionAttribute</c> resolves this on every authorized
    /// request, and a screen that loads five endpoints paid for five walks of the chain.
    /// The JWT still carries no permission claims, so a revoked grant takes effect as soon
    /// as the cache is signalled — no token has to be invalidated.
    /// </summary>
    public async Task<UserPermissions> GetForUserAsync(ulong userId, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey(userId), out UserPermissions? cached) && cached is not null)
        {
            return cached;
        }

        var permissions = await ResolveAsync(userId, ct);

        using var entry = _cache.CreateEntry(CacheKey(userId));
        entry.Value = permissions;
        entry.AbsoluteExpirationRelativeToNow = CacheLifetime;
        entry.AddExpirationToken(_signal.Token);

        return permissions;
    }

    private async Task<UserPermissions> ResolveAsync(ulong userId, CancellationToken ct)
    {
        var grants = await _query.GetGrantsForUserAsync((long)userId, ct);
        if (grants.FunctionIds.Count == 0)
        {
            return UserPermissions.Empty;
        }

        // Codes are typed by hand on the permission screen, so two rows differing only in
        // case are the same permission — de-duplicated here so the client never has to care.
        var codes = grants.Functions
                          .Select(g => g.FunctionCode)
                          .Where(code => !string.IsNullOrWhiteSpace(code))
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          .ToList();

        return new UserPermissions(grants.FunctionIds, codes);
    }

    private static string CacheKey(ulong userId) => $"permissions:{userId}";
}
