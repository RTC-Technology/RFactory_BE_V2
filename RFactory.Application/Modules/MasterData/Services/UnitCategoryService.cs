using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public interface IUnitCategoryService
{
    Task<List<UnitCategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<UnitCategoryDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<UnitCategoryDto>> CreateAsync(CreateUnitCategoryRequest request, CancellationToken ct = default);
    Task<Result<UnitCategoryDto>> UpdateAsync(ulong id, UpdateUnitCategoryRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IUnitConversionService
{
    Task<List<UnitConversionDto>> GetAllAsync(CancellationToken ct = default);
    Task<UnitConversionDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<UnitConversionDto>> CreateAsync(CreateUnitConversionRequest request, CancellationToken ct = default);
    Task<Result<UnitConversionDto>> UpdateAsync(ulong id, UpdateUnitConversionRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public class UnitCategoryService : IUnitCategoryService
{
    private readonly IRepository<UnitCategory> _repository;
    private readonly IRepository<Unit> _units;
    private readonly IMapper _mapper;

    public UnitCategoryService(IRepository<UnitCategory> repository, IRepository<Unit> units, IMapper mapper)
    {
        _repository = repository;
        _units = units;
        _mapper = mapper;
    }

    public async Task<List<UnitCategoryDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<UnitCategoryDto>>(await _repository.GetAll(ct));

    public async Task<UnitCategoryDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<UnitCategoryDto>(entity);
    }

    public async Task<Result<UnitCategoryDto>> CreateAsync(CreateUnitCategoryRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(c => c.UnitCategoryCode == request.UnitCategoryCode, ct);
        if (existing is not null)
        {
            return Result<UnitCategoryDto>.Failure($"Unit category code '{request.UnitCategoryCode}' already exists.");
        }

        var entity = _mapper.Map<UnitCategory>(request);
        await _repository.Add(entity, ct);
        return Result<UnitCategoryDto>.Success(_mapper.Map<UnitCategoryDto>(entity));
    }

    public async Task<Result<UnitCategoryDto>> UpdateAsync(ulong id, UpdateUnitCategoryRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<UnitCategoryDto>.Failure($"Unit category {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            c => c.Id != id && c.UnitCategoryCode == request.UnitCategoryCode, ct);
        if (existing is not null)
        {
            return Result<UnitCategoryDto>.Failure($"Unit category code '{request.UnitCategoryCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<UnitCategoryDto>.Success(_mapper.Map<UnitCategoryDto>(entity));
    }

    /// <summary>Refuses while units still sit in the category — soft delete does not
    /// cascade, so those units would keep pointing at a row that no longer reads.</summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Unit category {id} was not found.");
        }

        var categoryId = (long)id;
        var inUse = await _units.Where(u => u.UnitCategoryId == categoryId, ct);
        if (inUse.Count > 0)
        {
            return Result.Failure($"Unit category {id} still holds {inUse.Count} unit(s).");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}

public class UnitConversionService : IUnitConversionService
{
    private readonly IRepository<UnitConversion> _repository;
    private readonly IMapper _mapper;

    public UnitConversionService(IRepository<UnitConversion> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<UnitConversionDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<UnitConversionDto>>(await _repository.GetAll(ct));

    public async Task<UnitConversionDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<UnitConversionDto>(entity);
    }

    public async Task<Result<UnitConversionDto>> CreateAsync(CreateUnitConversionRequest request, CancellationToken ct = default)
    {
        var duplicate = await _repository.FirstOrDefault(
            c => c.FromUnitId == request.FromUnitId && c.ToUnitId == request.ToUnitId, ct);
        if (duplicate is not null)
        {
            return Result<UnitConversionDto>.Failure("A conversion between these two units already exists.");
        }

        var entity = _mapper.Map<UnitConversion>(request);
        await _repository.Add(entity, ct);
        return Result<UnitConversionDto>.Success(_mapper.Map<UnitConversionDto>(entity));
    }

    public async Task<Result<UnitConversionDto>> UpdateAsync(ulong id, UpdateUnitConversionRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<UnitConversionDto>.Failure($"Unit conversion {id} was not found.");
        }

        var duplicate = await _repository.FirstOrDefault(
            c => c.Id != id && c.FromUnitId == request.FromUnitId && c.ToUnitId == request.ToUnitId, ct);
        if (duplicate is not null)
        {
            return Result<UnitConversionDto>.Failure("A conversion between these two units already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<UnitConversionDto>.Success(_mapper.Map<UnitConversionDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Unit conversion {id} was not found.");
    }
}
