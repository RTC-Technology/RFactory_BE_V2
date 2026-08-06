using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// User groups plus the two link tables that hang off them: which functions a group
/// grants (UserGroupRightDistribution) and who belongs to it (UserGroupLink).
///
/// Both links are managed as whole sets rather than per-row, because that is how the
/// UI edits them — a checkbox list is saved in one go.
/// </summary>
public interface IUserGroupService
{
    Task<List<UserGroupDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<UserGroupDto>> CreateAsync(CreateUserGroupRequest request, CancellationToken ct = default);
    Task<Result<UserGroupDto>> UpdateAsync(ulong id, UpdateUserGroupRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);

    Task<List<long>> GetFunctionIdsAsync(ulong groupId, CancellationToken ct = default);
    Task<Result> SetFunctionsAsync(ulong groupId, IReadOnlyCollection<long> functionIds, CancellationToken ct = default);

    Task<List<long>> GetUserIdsAsync(ulong groupId, CancellationToken ct = default);
    Task<Result> SetUsersAsync(ulong groupId, IReadOnlyCollection<long> userIds, CancellationToken ct = default);
}
