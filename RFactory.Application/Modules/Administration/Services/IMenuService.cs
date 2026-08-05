using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// Application service for navigation menu items.
/// </summary>
public interface IMenuService
{
    Task<List<MenuDto>> GetAllAsync(CancellationToken ct = default);
    Task<MenuDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<MenuDto>> CreateAsync(CreateMenuRequest request, CancellationToken ct = default);
    Task<Result<MenuDto>> UpdateAsync(ulong id, UpdateMenuRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);

    /// <summary>
    /// Returns a pre-built menu tree filtered by the user's access level.
    /// Admins see all items; regular users see only items with no FunctionId
    /// (public menus) until UserGroup-based permissions are wired up.
    /// </summary>
    Task<List<MenuDto>> GetMenusForUserAsync(ulong userId, bool isAdmin, CancellationToken ct = default);
}
