namespace RFactory.Application.Modules.Administration.DTOs;

/// <summary>
/// Read model for a user group — the unit that carries permissions. Users reach a
/// function through UserGroupLink → UserGroup → UserGroupRightDistribution.
/// </summary>
public class UserGroupDto
{
    public ulong Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Payload for creating a user group.
/// </summary>
public class CreateUserGroupRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Payload for updating a user group.
/// </summary>
public class UpdateUserGroupRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Replaces the group's whole set of functions. The list is authoritative: anything
/// missing from it is unassigned, so the caller sends the full selection, not a delta.
/// </summary>
public class SetUserGroupFunctionsRequest
{
    public List<long> FunctionIds { get; set; } = new();
}

/// <summary>
/// Replaces the group's whole membership, with the same all-or-nothing semantics as
/// <see cref="SetUserGroupFunctionsRequest"/>.
/// </summary>
public class SetUserGroupUsersRequest
{
    public List<long> UserIds { get; set; } = new();
}
