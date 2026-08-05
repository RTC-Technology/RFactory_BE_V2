namespace RFactory.Application.Modules.Administration.DTOs;

/// <summary>
/// Read model returned to API clients for a menu item.
/// Children is populated only by the tree endpoint (GET /api/auth/menus).
/// The flat CRUD endpoints leave Children null.
/// </summary>
public class MenuDto
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? Order { get; set; }
    public long? ParentId { get; set; }
    public long? FunctionId { get; set; }
    public List<MenuDto>? Children { get; set; }
}

/// <summary>
/// Payload for creating a menu item.
/// </summary>
public class CreateMenuRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? Order { get; set; }
    public long? ParentId { get; set; }
    public long? FunctionId { get; set; }
}

/// <summary>
/// Payload for updating a menu item.
/// </summary>
public class UpdateMenuRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? Order { get; set; }
    public long? ParentId { get; set; }
    public long? FunctionId { get; set; }
}
