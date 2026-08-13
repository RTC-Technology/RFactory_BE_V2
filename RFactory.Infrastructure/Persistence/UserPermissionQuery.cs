using Microsoft.EntityFrameworkCore;
using RFactory.Infrastructure.Data;

namespace RFactory.Infrastructure.Persistence;

/// <inheritdoc cref="IUserPermissionQuery"/>
public sealed class UserPermissionQuery : IUserPermissionQuery
{
    private readonly RFactoryContext _context;

    public UserPermissionQuery(RFactoryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserGrants> GetGrantsForUserAsync(long userId, CancellationToken ct = default)
    {
        // Two round trips, not one. Function.Id is ulong while UserGroupRightDistribution
        // .FunctionId is long?, so joining them directly would push a CAST into the join
        // predicate and cost the index on functions.id. The ids are converted in memory
        // between the two queries instead, which keeps the second one a plain SQL IN.
        var functionIds = await (
            from link in _context.UserGroupLinks
            join right in _context.UserGroupRightDistributions
                on link.UserGroupId equals right.UserGroupId
            where link.UserId == userId && right.FunctionId.HasValue
            select right.FunctionId!.Value)
            .Distinct()
            .ToListAsync(ct);

        if (functionIds.Count == 0)
        {
            return UserGrants.Empty;
        }

        var lookupIds = functionIds.Select(id => (ulong)id).ToList();

        // Projected to an anonymous type and widened afterwards: putting the (long) cast
        // inside the Select would hand EF one more expression to translate into SQL, which
        // is the very thing the two-query split exists to avoid.
        var rows = await _context.Functions
            .AsNoTracking()
            .Where(f => lookupIds.Contains(f.Id))
            .Select(f => new { f.Id, f.FunctionCode })
            .ToListAsync(ct);

        var functions = rows
            .Select(row => new PermissionGrant((long)row.Id, row.FunctionCode))
            .ToList();

        // The id set is the *unresolved* one on purpose — see UserGrants.FunctionIds.
        return new UserGrants(functionIds.ToHashSet(), functions);
    }
}
