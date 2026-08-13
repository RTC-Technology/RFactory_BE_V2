namespace RFactory.Infrastructure.Persistence;

/// <summary>One resolved function: the code an endpoint or the client checks against.</summary>
public readonly record struct PermissionGrant(long FunctionId, string FunctionCode);

/// <summary>
/// What a user's groups grant, in the two shapes the callers need.
/// </summary>
/// <param name="FunctionIds">
/// Every function id on the user's right rows, resolved or not. Deleting a Function does not
/// cascade to <c>user_group_right_distribution</c>, so a right can outlive the row it points
/// at; those ids stay here because menu gating has always matched on the raw id.
/// </param>
/// <param name="Functions">
/// Only the rights whose Function row still exists — the ones that can yield a code.
/// </param>
public sealed record UserGrants(IReadOnlySet<long> FunctionIds, IReadOnlyList<PermissionGrant> Functions)
{
    public static UserGrants Empty { get; } =
        new(new HashSet<long>(), Array.Empty<PermissionGrant>());
}

/// <summary>
/// The one read that walks User → UserGroupLink → UserGroupRightDistribution → Function.
///
/// Exists as its own contract rather than going through <see cref="IRepository{T}"/> because
/// that one deliberately returns materialised lists and never exposes <c>IQueryable</c>, so a
/// join cannot be expressed through it. Resolving this chain runs on *every* authorized
/// request, which is exactly where three sequential round trips stop being acceptable.
///
/// Lives in Infrastructure next to the entities for the same reason <see cref="IRepository{T}"/>
/// does: the Application layer depends on Infrastructure in a single direction, and pulling
/// this up would need the entities to come with it.
/// </summary>
public interface IUserPermissionQuery
{
    /// <summary>
    /// Grants held by <paramref name="userId"/>. Empty when the user is in no group or their
    /// groups grant nothing. Soft-deleted rows are excluded by the global query filter, so a
    /// revoked grant never shows up here.
    /// </summary>
    Task<UserGrants> GetGrantsForUserAsync(long userId, CancellationToken ct = default);
}
