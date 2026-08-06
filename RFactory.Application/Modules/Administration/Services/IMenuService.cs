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
    /// Returns a pre-built menu tree filtered by what the user may reach.
    ///
    /// Admins see everything. For everyone else an item with no FunctionId is public,
    /// and an item that carries one is shown only when the user's groups grant that
    /// function. Denying a parent hides its children with it, and a group left with no
    /// children is dropped rather than rendered as a header that expands into nothing.
    /// </summary>
    Task<List<MenuDto>> GetMenusForUserAsync(ulong userId, bool isAdmin, CancellationToken ct = default);
}
