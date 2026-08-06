using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _repository;
    private readonly IUserPermissionService _permissions;
    private readonly IMapper _mapper;

    public MenuService(
        IRepository<Menu> repository,
        IUserPermissionService permissions,
        IMapper mapper)
    {
        _repository = repository;
        _permissions = permissions;
        _mapper = mapper;
    }

    public async Task<List<MenuDto>> GetAllAsync(CancellationToken ct = default)
    {
        var menus = await _repository.GetAll(ct);
        return _mapper.Map<List<MenuDto>>(menus);
    }

    public async Task<MenuDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var menu = await _repository.GetById(id, ct);
        return menu is null ? null : _mapper.Map<MenuDto>(menu);
    }

    public async Task<Result<MenuDto>> CreateAsync(CreateMenuRequest request, CancellationToken ct = default)
    {
        var menu = _mapper.Map<Menu>(request);
        await _repository.Add(menu, ct);
        return Result<MenuDto>.Success(_mapper.Map<MenuDto>(menu));
    }

    public async Task<Result<MenuDto>> UpdateAsync(ulong id, UpdateMenuRequest request, CancellationToken ct = default)
    {
        var menu = await _repository.GetById(id, ct);
        if (menu is null)
        {
            return Result<MenuDto>.Failure($"Menu {id} was not found.");
        }

        _mapper.Map(request, menu);
        await _repository.Update(menu, ct);
        return Result<MenuDto>.Success(_mapper.Map<MenuDto>(menu));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Menu {id} was not found.");
    }

    public async Task<List<MenuDto>> GetMenusForUserAsync(ulong userId, bool isAdmin, CancellationToken ct = default)
    {
        var menus = await _repository.GetAll(ct);

        if (!isAdmin)
        {
            // A null FunctionId is an open door — the item is public and needs no grant.
            // Anything else is only visible to a user whose groups hand them that
            // function. Filtered in memory because the tree has to be assembled from the
            // full set anyway, and the menu table is small.
            var granted = (await _permissions.GetForUserAsync(userId, ct)).FunctionIds;
            menus = menus.Where(m => !m.FunctionId.HasValue || granted.Contains(m.FunctionId.Value)).ToList();

            // Denying a parent also hides its children: they keep a ParentId that is no
            // longer in the set, so BuildTree never reaches them from the root. That is
            // the behaviour we want, and it is load-bearing — not an accident.
            var dtos = BuildTree(_mapper.Map<List<MenuDto>>(menus));
            return PruneEmptyGroups(dtos);
        }

        return BuildTree(_mapper.Map<List<MenuDto>>(menus));
    }

    /// <summary>
    /// Drops group items that ended up with nothing under them. A group carries no URL of
    /// its own, so once filtering has taken every child away it is a header that expands
    /// into nothing. Admins keep seeing it through the CRUD endpoints.
    /// </summary>
    private static List<MenuDto> PruneEmptyGroups(List<MenuDto> nodes)
    {
        var kept = new List<MenuDto>();
        foreach (var node in nodes)
        {
            if (node.Children is { Count: > 0 })
            {
                var children = PruneEmptyGroups(node.Children);
                node.Children = children.Count > 0 ? children : null;
            }

            var isEmptyGroup = string.IsNullOrWhiteSpace(node.Url) && node.Children is null or { Count: 0 };
            if (!isEmptyGroup)
            {
                kept.Add(node);
            }
        }

        return kept;
    }

    /// <summary>
    /// Converts a flat list of MenuDto items into a tree sorted by Order.
    /// Root items (ParentId == null) form the top level; each item's Children
    /// list contains its direct descendants, recursively sorted.
    /// </summary>
    private static List<MenuDto> BuildTree(List<MenuDto> flat)
    {
        var lookup = flat.ToLookup(m => m.ParentId);

        List<MenuDto> Attach(long? parentId)
        {
            var children = lookup[parentId]
                .OrderBy(m => m.Order ?? int.MaxValue)
                .ToList();

            foreach (var child in children)
            {
                var nested = Attach((long)child.Id);
                child.Children = nested.Count > 0 ? nested : null;
            }

            return children;
        }

        return Attach(null);
    }
}
