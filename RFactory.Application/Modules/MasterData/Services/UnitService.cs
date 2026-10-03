using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using static RFactory.Shared.Constants.PermissionCodes;
using Entities = RFactory.Infrastructure.Entities;
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
    Task<Result<UnitDto>> CreateAsync(UnitRequest request, CancellationToken ct = default);
    Task<Result<UnitDto>> UpdateAsync(ulong id, UnitRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public class UnitService : IUnitService
{
    private readonly IRepository<Entities.Unit> _repository;
    private readonly IRepository<ProductEntity> _products;
    private readonly IRepository<Entities.BomDetail> _bomDetails;
    private readonly IRepository<ProductUnit> _productUnits;
    private readonly IRepository<Entities.UnitConversion> _conversions;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(
        IRepository<Entities.Unit> repository,
        IRepository<ProductEntity> products,
        IRepository<Entities.BomDetail> bomDetails,
        IRepository<ProductUnit> productUnits,
        IRepository<Entities.UnitConversion> conversions,
        IUnitOfWork unitOfWork,
    IMapper mapper)
    {
        _repository = repository;
        _products = products;
        _bomDetails = bomDetails;
        _productUnits = productUnits;
        _conversions = conversions;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<UnitDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<UnitDto>>(await _repository.GetAll(ct));

    public async Task<UnitDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<UnitDto>(entity);
    }

    public async Task<Result<UnitDto>> CreateAsync(UnitRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(t => t.UnitCode == request.UnitCode, ct);
        if (existing is not null)
        {
            return Result<UnitDto>.Failure($"Unit '{request.UnitCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.Unit>(request);
        var lines = request.UnitConversions ?? new List<UnitConversionRequest>();
        return await _unitOfWork.ExecuteAsync(async token =>
        {
            // Two saves rather than one: the lines need the id the database generates for
            // the receipt, which is only known once the receipt is in.

            await _repository.Add(entity, token);
            await _conversions.AddRange(lines.Select(line => ToLineEntity(line, entity.Id)).ToList(), token);

            return Result<UnitDto>.Success(_mapper.Map<UnitDto>(entity));
        }, ct);
    }

    public async Task<Result<UnitDto>> UpdateAsync(ulong id, UnitRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<UnitDto>.Failure($"Unit {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            t => t.Id != id && t.UnitCode == request.UnitCode, ct);
        if (existing is not null)
        {
            return Result<UnitDto>.Failure($"Unit '{request.UnitCode}' already exists.");
        }

        var stored = await _conversions.Where(l => l.FromUnitId == (long)id, ct);

        var lines = request.UnitConversions;
        var keptIds = (lines ?? new List<UnitConversionRequest>())
            .Where(line => line.Id != 0)
            .Select(line => line.Id)
            .ToHashSet();

        // The list replaces the whole set, so an id from another receipt would be edited
        // here and dropped from where it belongs. Reject the payload instead.
        var foreign = keptIds.Where(lineId => stored.All(s => s.Id != lineId)).ToList();
        if (foreign.Count > 0)
        {
            return Result<UnitDto>.Failure(
                $"Line(s) {string.Join(", ", foreign)} do not belong to Unit {id}.");
        }

        _mapper.Map(request, entity);

        return await _unitOfWork.ExecuteAsync(async token =>
        {
            await _repository.Update(entity, token);

            // A null list means the caller is editing the header only; an empty one means
            // the receipt really has no lines left.
            if (lines is not null)
            {
                await _conversions.DeleteRange(
                    stored.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                foreach (var line in lines.Where(l => l.Id != 0))
                {
                    var target = stored.First(s => s.Id == line.Id);
                    _mapper.Map(line, target);
                    await _conversions.Update(target, token);
                }

                await _conversions.AddRange(
                    lines.Where(l => l.Id == 0).Select(line => ToLineEntity(line, id)).ToList(), token);

            }

            return Result<UnitDto>.Success(_mapper.Map<UnitDto>(entity));
        }, ct);
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


    private Entities.UnitConversion ToLineEntity(UnitConversionRequest line, ulong id)
    {
        var entity = _mapper.Map<Entities.UnitConversion>(line);
        entity.FromUnitId = (long)id;
        return entity;
    }
}
