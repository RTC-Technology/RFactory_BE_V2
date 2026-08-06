namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// What a single user is allowed to do, resolved once and used two ways: the function
/// ids gate menu items, the function codes are what the client checks against.
/// </summary>
public sealed record UserPermissions(IReadOnlySet<long> FunctionIds, IReadOnlyList<string> Codes)
{
    public static UserPermissions Empty { get; } =
        new(new HashSet<long>(), Array.Empty<string>());
}

/// <summary>
/// Walks User → UserGroupLink → UserGroupRightDistribution → Function.
///
/// Admins are not special-cased here: this reports what a user was actually granted.
/// Callers that let <c>IsAdmin</c> bypass everything apply that themselves, so the
/// distinction between "granted" and "bypassed" never gets lost in the data.
/// </summary>
public interface IUserPermissionService
{
    Task<UserPermissions> GetForUserAsync(ulong userId, CancellationToken ct = default);
}
