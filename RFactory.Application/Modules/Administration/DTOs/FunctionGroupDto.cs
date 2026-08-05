namespace RFactory.Application.Modules.Administration.DTOs;

/// <summary>
/// Read model returned to API clients for a function group (permission group).
/// </summary>
public class FunctionGroupDto
{
    public ulong Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}

/// <summary>
/// Payload for creating a function group.
/// </summary>
public class CreateFunctionGroupRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}

/// <summary>
/// Payload for updating a function group.
/// </summary>
public class UpdateFunctionGroupRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}
