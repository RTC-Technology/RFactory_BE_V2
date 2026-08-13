using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

public class UserGroupService : IUserGroupService
{
    private readonly IRepository<UserGroup> _groups;
    private readonly IRepository<UserGroupRightDistribution> _rights;
    private readonly IRepository<UserGroupLink> _links;
    private readonly IMapper _mapper;
    private readonly IPermissionCacheSignal _permissionCache;

    public UserGroupService(
        IRepository<UserGroup> groups,
        IRepository<UserGroupRightDistribution> rights,
        IRepository<UserGroupLink> links,
        IMapper mapper,
        IPermissionCacheSignal permissionCache)
    {
        _groups = groups;
        _rights = rights;
        _links = links;
        _mapper = mapper;
        _permissionCache = permissionCache;
    }

    public async Task<List<UserGroupDto>> GetAllAsync(CancellationToken ct = default)
    {
        var groups = await _groups.GetAll(ct);
        return _mapper.Map<List<UserGroupDto>>(groups);
    }

    public async Task<UserGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var group = await _groups.GetById(id, ct);
        return group is null ? null : _mapper.Map<UserGroupDto>(group);
    }

    public async Task<Result<UserGroupDto>> CreateAsync(CreateUserGroupRequest request, CancellationToken ct = default)
    {
        var existing = await _groups.FirstOrDefault(g => g.Code == request.Code, ct);
        if (existing is not null)
        {
            return Result<UserGroupDto>.Failure($"User group code '{request.Code}' already exists.");
        }

        var group = _mapper.Map<UserGroup>(request);
        await _groups.Add(group, ct);
        return Result<UserGroupDto>.Success(_mapper.Map<UserGroupDto>(group));
    }

    public async Task<Result<UserGroupDto>> UpdateAsync(ulong id, UpdateUserGroupRequest request, CancellationToken ct = default)
    {
        var group = await _groups.GetById(id, ct);
        if (group is null)
        {
            return Result<UserGroupDto>.Failure($"User group {id} was not found.");
        }

        var existing = await _groups.FirstOrDefault(g => g.Id != id && g.Code == request.Code, ct);
        if (existing is not null)
        {
            return Result<UserGroupDto>.Failure($"User group code '{request.Code}' already exists.");
        }

        _mapper.Map(request, group);
        await _groups.Update(group, ct);
        return Result<UserGroupDto>.Success(_mapper.Map<UserGroupDto>(group));
    }

    /// <summary>
    /// Deletes the group and both of its link sets. Nothing else references a group, and
    /// leaving the links behind would strand rows pointing at an id that no longer reads.
    /// </summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var group = await _groups.GetById(id, ct);
        if (group is null)
        {
            return Result.Failure($"User group {id} was not found.");
        }

        var groupId = (long)id;
        await _rights.DeleteRange(await _rights.Where(r => r.UserGroupId == groupId, ct), ct);
        await _links.DeleteRange(await _links.Where(l => l.UserGroupId == groupId, ct), ct);
        await _groups.Delete(group, ct);

        _permissionCache.Invalidate();
        return Result.Success();
    }

    // ─── Functions granted by the group ──────────────────────────────────────────

    public async Task<List<long>> GetFunctionIdsAsync(ulong groupId, CancellationToken ct = default)
    {
        var id = (long)groupId;
        var rows = await _rights.Where(r => r.UserGroupId == id, ct);
        return rows.Where(r => r.FunctionId.HasValue).Select(r => r.FunctionId!.Value).Distinct().ToList();
    }

    public async Task<Result> SetFunctionsAsync(ulong groupId, IReadOnlyCollection<long> functionIds, CancellationToken ct = default)
    {
        if (await _groups.GetById(groupId, ct) is null)
        {
            return Result.Failure($"User group {groupId} was not found.");
        }

        var id = (long)groupId;
        var current = await _rights.Where(r => r.UserGroupId == id, ct);
        var target = functionIds.ToHashSet();

        // Rows with a null FunctionId grant nothing, so they are swept out alongside the
        // ones the caller dropped.
        var stale = current.Where(r => !r.FunctionId.HasValue || !target.Contains(r.FunctionId.Value)).ToList();
        var kept = current.Where(r => r.FunctionId.HasValue && target.Contains(r.FunctionId.Value))
                          .Select(r => r.FunctionId!.Value)
                          .ToHashSet();

        var added = target.Except(kept)
            .Select(functionId => new UserGroupRightDistribution { UserGroupId = id, FunctionId = functionId })
            .ToList();

        await _rights.DeleteRange(stale, ct);
        await _rights.AddRange(added, ct);

        _permissionCache.Invalidate();
        return Result.Success();
    }

    // ─── Members of the group ────────────────────────────────────────────────────

    public async Task<List<long>> GetUserIdsAsync(ulong groupId, CancellationToken ct = default)
    {
        var id = (long)groupId;
        var rows = await _links.Where(l => l.UserGroupId == id, ct);
        return rows.Where(l => l.UserId.HasValue).Select(l => l.UserId!.Value).Distinct().ToList();
    }

    public async Task<Result> SetUsersAsync(ulong groupId, IReadOnlyCollection<long> userIds, CancellationToken ct = default)
    {
        if (await _groups.GetById(groupId, ct) is null)
        {
            return Result.Failure($"User group {groupId} was not found.");
        }

        var id = (long)groupId;
        var current = await _links.Where(l => l.UserGroupId == id, ct);
        var target = userIds.ToHashSet();

        var stale = current.Where(l => !l.UserId.HasValue || !target.Contains(l.UserId.Value)).ToList();
        var kept = current.Where(l => l.UserId.HasValue && target.Contains(l.UserId.Value))
                          .Select(l => l.UserId!.Value)
                          .ToHashSet();

        var added = target.Except(kept)
            .Select(userId => new UserGroupLink { UserGroupId = id, UserId = userId })
            .ToList();

        await _links.DeleteRange(stale, ct);
        await _links.AddRange(added, ct);

        _permissionCache.Invalidate();
        return Result.Success();
    }
}
