using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;

namespace RFactory.Application.Modules.Administration.Services;

public class UserPermissionService : IUserPermissionService
{
    private readonly IRepository<UserGroupLink> _links;
    private readonly IRepository<UserGroupRightDistribution> _rights;
    private readonly IRepository<Function> _functions;

    public UserPermissionService(
        IRepository<UserGroupLink> links,
        IRepository<UserGroupRightDistribution> rights,
        IRepository<Function> functions)
    {
        _links = links;
        _rights = rights;
        _functions = functions;
    }

    public async Task<UserPermissions> GetForUserAsync(ulong userId, CancellationToken ct = default)
    {
        var id = (long)userId;

        var links = await _links.Where(l => l.UserId == id, ct);
        var groupIds = links.Where(l => l.UserGroupId.HasValue)
                            .Select(l => l.UserGroupId!.Value)
                            .Distinct()
                            .ToList();
        if (groupIds.Count == 0)
        {
            return UserPermissions.Empty;
        }

        var rights = await _rights.Where(r => r.UserGroupId.HasValue && groupIds.Contains(r.UserGroupId.Value), ct);
        var functionIds = rights.Where(r => r.FunctionId.HasValue)
                                .Select(r => r.FunctionId!.Value)
                                .Distinct()
                                .ToHashSet();
        if (functionIds.Count == 0)
        {
            return UserPermissions.Empty;
        }

        // Function.Id is ulong while the link column is long?; the list is converted up
        // front so the Contains translates to a plain SQL IN rather than a cast EF would
        // have to push into the query.
        var lookupIds = functionIds.Select(fid => (ulong)fid).ToList();
        var functions = await _functions.Where(f => lookupIds.Contains(f.Id), ct);

        var codes = functions.Select(f => f.FunctionCode)
                             .Where(code => !string.IsNullOrWhiteSpace(code))
                             .Distinct(StringComparer.OrdinalIgnoreCase)
                             .ToList();

        return new UserPermissions(functionIds, codes);
    }
}
