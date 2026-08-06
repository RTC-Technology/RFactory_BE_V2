using AutoMapper;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Product.Services;

public class ProductTypeService : IProductTypeService
{
    private readonly IRepository<Entities.ProductType> _repository;
    private readonly IRepository<Entities.Product> _products;
    private readonly IMapper _mapper;

    public ProductTypeService(
        IRepository<Entities.ProductType> repository,
        IRepository<Entities.Product> products,
        IMapper mapper)
    {
        _repository = repository;
        _products = products;
        _mapper = mapper;
    }

    public async Task<List<ProductTypeDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<ProductTypeDto>>(await _repository.GetAll(ct));

    public async Task<ProductTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<ProductTypeDto>(entity);
    }

    public async Task<Result<ProductTypeDto>> CreateAsync(CreateProductTypeRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(t => t.ProductTypeCode == request.ProductTypeCode, ct);
        if (existing is not null)
        {
            return Result<ProductTypeDto>.Failure($"Product type code '{request.ProductTypeCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.ProductType>(request);
        await _repository.Add(entity, ct);
        return Result<ProductTypeDto>.Success(_mapper.Map<ProductTypeDto>(entity));
    }

    public async Task<Result<ProductTypeDto>> UpdateAsync(ulong id, UpdateProductTypeRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<ProductTypeDto>.Failure($"Product type {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            t => t.Id != id && t.ProductTypeCode == request.ProductTypeCode, ct);
        if (existing is not null)
        {
            return Result<ProductTypeDto>.Failure($"Product type code '{request.ProductTypeCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<ProductTypeDto>.Success(_mapper.Map<ProductTypeDto>(entity));
    }

    /// <summary>
    /// Refuses while products still reference the type. Delete is a non-cascading soft
    /// delete, so those products would keep a ProductTypeId that no longer reads.
    /// </summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Product type {id} was not found.");
        }

        var typeId = (long)id;
        var inUse = await _products.Where(p => p.ProductTypeId == typeId, ct);
        if (inUse.Count > 0)
        {
            return Result.Failure($"Product type {id} is still used by {inUse.Count} product(s).");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}

public class ProductService : IProductService
{
    private readonly IRepository<Entities.Product> _repository;
    private readonly IRepository<Entities.Bom> _boms;
    private readonly IRepository<Entities.BomDetail> _bomDetails;
    private readonly IMapper _mapper;

    public ProductService(
        IRepository<Entities.Product> repository,
        IRepository<Entities.Bom> boms,
        IRepository<Entities.BomDetail> bomDetails,
        IMapper mapper)
    {
        _repository = repository;
        _boms = boms;
        _bomDetails = bomDetails;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<ProductDto>>(await _repository.GetAll(ct));

    public async Task<ProductDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<ProductDto>(entity);
    }

    public async Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(p => p.ProductCode == request.ProductCode, ct);
        if (existing is not null)
        {
            return Result<ProductDto>.Failure($"Product code '{request.ProductCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.Product>(request);
        await _repository.Add(entity, ct);
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(entity));
    }

    public async Task<Result<ProductDto>> UpdateAsync(ulong id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<ProductDto>.Failure($"Product {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            p => p.Id != id && p.ProductCode == request.ProductCode, ct);
        if (existing is not null)
        {
            return Result<ProductDto>.Failure($"Product code '{request.ProductCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(entity));
    }

    /// <summary>
    /// Refuses while the product owns a BOM, or appears as a component inside somebody
    /// else's BOM — deleting it would leave either set pointing at a row that is gone.
    /// </summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Product {id} was not found.");
        }

        var productId = (long)id;

        var ownBoms = await _boms.Where(b => b.ProductId == productId, ct);
        if (ownBoms.Count > 0)
        {
            return Result.Failure($"Product {id} still owns {ownBoms.Count} BOM(s). Remove them first.");
        }

        var usedAsComponent = await _bomDetails.Where(d => d.ProductId == productId, ct);
        if (usedAsComponent.Count > 0)
        {
            return Result.Failure($"Product {id} is used as a component in {usedAsComponent.Count} BOM line(s).");
        }

        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}

public class BomService : IBomService
{
    private readonly IRepository<Entities.Bom> _repository;
    private readonly IRepository<Entities.BomDetail> _details;
    private readonly IMapper _mapper;

    public BomService(
        IRepository<Entities.Bom> repository,
        IRepository<Entities.BomDetail> details,
        IMapper mapper)
    {
        _repository = repository;
        _details = details;
        _mapper = mapper;
    }

    public async Task<List<BomDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<BomDto>>(await _repository.GetAll(ct));

    public async Task<BomDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<BomDto>(entity);
    }

    public async Task<Result<BomDto>> CreateAsync(CreateBomRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(b => b.BomCode == request.BomCode, ct);
        if (existing is not null)
        {
            return Result<BomDto>.Failure($"BOM code '{request.BomCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.Bom>(request);
        await _repository.Add(entity, ct);
        return Result<BomDto>.Success(_mapper.Map<BomDto>(entity));
    }

    public async Task<Result<BomDto>> UpdateAsync(ulong id, UpdateBomRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<BomDto>.Failure($"BOM {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(b => b.Id != id && b.BomCode == request.BomCode, ct);
        if (existing is not null)
        {
            return Result<BomDto>.Failure($"BOM code '{request.BomCode}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<BomDto>.Success(_mapper.Map<BomDto>(entity));
    }

    /// <summary>Deletes the BOM together with its lines — the lines exist only for it.</summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"BOM {id} was not found.");
        }

        var bomId = (long)id;
        await _details.DeleteRange(await _details.Where(d => d.BomId == bomId, ct), ct);
        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}

public class BomDetailService : IBomDetailService
{
    private readonly IRepository<Entities.BomDetail> _repository;
    private readonly IMapper _mapper;

    public BomDetailService(IRepository<Entities.BomDetail> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<BomDetailDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<BomDetailDto>>(await _repository.GetAll(ct));

    public async Task<BomDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<BomDetailDto>(entity);
    }

    public async Task<Result<BomDetailDto>> CreateAsync(CreateBomDetailRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.BomDetail>(request);
        await _repository.Add(entity, ct);
        return Result<BomDetailDto>.Success(_mapper.Map<BomDetailDto>(entity));
    }

    public async Task<Result<BomDetailDto>> UpdateAsync(ulong id, UpdateBomDetailRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<BomDetailDto>.Failure($"BOM line {id} was not found.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<BomDetailDto>.Success(_mapper.Map<BomDetailDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"BOM line {id} was not found.");
    }
}
