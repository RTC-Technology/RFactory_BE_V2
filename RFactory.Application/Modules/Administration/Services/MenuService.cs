using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _repository;
    private readonly IMapper _mapper;

    public MenuService(IRepository<Menu> repository, IMapper mapper)
    {
        _repository = repository;
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
        List<Menu> menus;
        if (isAdmin)
        {
            menus = await _repository.GetAll(ct);
        }
        else
        {
            // Public menus: no FunctionId means no permission gate.
            // When UserGroup-based rights are wired up, this predicate will be expanded.
            menus = await _repository.Where(m => m.FunctionId == null, ct);
        }

        var dtos = _mapper.Map<List<MenuDto>>(menus);
        return BuildTree(dtos);
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
