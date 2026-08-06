using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
// `Product` also names the Application.Modules.Product namespace, so the entity is
// reached through an alias rather than the plain type name.
using ProductEntity = RFactory.Infrastructure.Entities.Product;

namespace RFactory.Application.Modules.MasterData.Services;

/// <summary>Units of measure. Currently read-only from the UI, but the full CRUD surface
/// is here so a declaration screen can be added without touching the backend.</summary>
public interface IUnitService
{
    Task<List<UnitDto>> GetAllAsync(CancellationToken ct = default);
    Task<UnitDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<UnitDto>> CreateAsync(CreateUnitRequest request, CancellationToken ct = default);
    Task<Result<UnitDto>> UpdateAsync(ulong id, UpdateUnitRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public class UnitService : IUnitService
{
    private readonly IRepository<Unit> _repository;
    private readonly IRepository<ProductEntity> _products;
    private readonly IRepository<BomDetail> _bomDetails;
    private readonly IRepository<ProductUnit> _productUnits;
    private readonly IRepository<UnitConversion> _conversions;
    private readonly IMapper _mapper;

    public UnitService(
        IRepository<Unit> repository,
        IRepository<ProductEntity> products,
        IRepository<BomDetail> bomDetails,
        IRepository<ProductUnit> productUnits,
        IRepository<UnitConversion> conversions,
        IMapper mapper)
    {
        _repository = repository;
        _products = products;
        _bomDetails = bomDetails;
        _productUnits = productUnits;
        _conversions = conversions;
        _mapper = mapper;
    }

    public async Task<List<UnitDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<UnitDto>>(await _repository.GetAll(ct));

    public async Task<UnitDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<UnitDto>(entity);
    }

    public async Task<Result<UnitDto>> CreateAsync(CreateUnitRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(u => u.UnitCode == request.UnitCode, ct);
        if (existing is not null)
        {
            return Result<UnitDto>.Failure($"Unit code '{request.UnitCode}' already exists.");
        }

        var entity = _mapper.Map<Unit>(request);
        await _repository.Add(entity, ct);
        return Result<UnitDto>.Success(_mapper.Map<UnitDto>(entity));
    }

    public async Task<Result<UnitDto>> UpdateAsync(ulong id, UpdateUnitRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<UnitDto>.Failure($"Unit {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(u => u.Id != id && u.UnitCode == request.UnitCode, ct);
        if (existing is not null)
        {
            return Result<UnitDto>.Failure($"Unit code '{request.UnitCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<UnitDto>.Success(_mapper.Map<UnitDto>(entity));
    }

    /// <summary>
    /// Refuses while anything still points at the unit. Soft delete does not cascade, so
    /// a product or BOM line left holding the id would show a blank unit with no clue why.
    /// </summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Unit {id} was not found.");
        }

        var unitId = (long)id;

        var products = await _products.Where(p => p.DefaultUnitId == unitId, ct);
        if (products.Count > 0)
        {
            return Result.Failure($"Unit {id} is the default unit of {products.Count} product(s).");
        }

        var lines = await _bomDetails.Where(d => d.UnitId == unitId, ct);
        if (lines.Count > 0)
        {
            return Result.Failure($"Unit {id} is used by {lines.Count} BOM line(s).");
        }

        var productUnits = await _productUnits.Where(pu => pu.UnitId == unitId, ct);
        if (productUnits.Count > 0)
        {
            return Result.Failure($"Unit {id} is used by {productUnits.Count} product-unit row(s).");
        }

        var conversions = await _conversions.Where(c => c.FromUnitId == unitId || c.ToUnitId == unitId, ct);
        if (conversions.Count > 0)
        {
            return Result.Failure($"Unit {id} is used by {conversions.Count} conversion(s).");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}
