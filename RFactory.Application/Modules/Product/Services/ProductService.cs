using AutoMapper;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Infrastructure.Entities;
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

public class ProductGroupService : IProductGroupService
{
    private readonly IRepository<Entities.ProductGroup> _repository;
    private readonly IRepository<Entities.Product> _products;
    private readonly IMapper _mapper;

    public ProductGroupService(
        IRepository<Entities.ProductGroup> repository,
        IRepository<Entities.Product> products,
        IMapper mapper)
    {
        _repository = repository;
        _products = products;
        _mapper = mapper;
    }

    public async Task<List<ProductGroupDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<ProductGroupDto>>(await _repository.GetAll(ct));

    public async Task<ProductGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<ProductGroupDto>(entity);
    }

    public async Task<Result<ProductGroupDto>> CreateAsync(ProductGroupRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(p => p.GroupNo == request.GroupNo, ct);
        if (existing is not null)
        {
            return Result<ProductGroupDto>.Failure($"Group no '{request.GroupNo}' already exists.");
        }

        var entity = _mapper.Map<Entities.ProductGroup>(request);
        await _repository.Add(entity, ct);
        return Result<ProductGroupDto>.Success(_mapper.Map<ProductGroupDto>(entity));
    }

    public async Task<Result<ProductGroupDto>> UpdateAsync(ulong id, ProductGroupRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<ProductGroupDto>.Failure($"Product {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(p => p.Id != id && p.GroupNo == request.GroupNo, ct);
        if (existing is not null)
        {
            return Result<ProductGroupDto>.Failure($"Group no '{request.GroupNo}' already exists.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<ProductGroupDto>.Success(_mapper.Map<ProductGroupDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Product group {id} was not found.");
        }

        var inUse = await _products.Where(p => p.ProductGroupId == (long)id, ct);
        if (inUse.Count > 0)
        {
            return Result.Failure($"Product group {id} is still used by {inUse.Count} product(s).");
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

    private readonly IRepository<Entities.Routing> _routing;
    private readonly IRepository<Entities.RoutingOperation> _routingOp;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(
        IRepository<Entities.Product> repository,
        IRepository<Entities.Bom> boms,
        IRepository<Entities.BomDetail> bomDetails,
        IRepository<Entities.Routing> routing,
        IRepository<Entities.RoutingOperation> routingOp,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _boms = boms;
        _bomDetails = bomDetails;
        _routing = routing;
        _routingOp = routingOp;

        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<ProductDto>>(await _repository.GetAll(ct));

    public async Task<ProductDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<ProductDto>(entity);
    }

    public async Task<Result<ProductDto>> CreateAsync(ProductRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(t => t.ProductCode == request.ProductCode, ct);
        if (existing is not null)
        {
            return Result<ProductDto>.Failure($"Product code '{request.ProductCode}' already exists.");
        }

        var entity = _mapper.Map<Entities.Product>(request);
        var bomRequests = request.Boms ?? new List<BomRequest>();
        var routingRequests = request.Routings ?? new List<RoutingRequest>();

        return await _unitOfWork.ExecuteAsync(async token =>
        {
            // Two saves rather than one: the lines need the id the database generates for
            // the receipt, which is only known once the receipt is in.
            await _repository.Add(entity, token);

            // 2. Map + insert bom
            foreach (var r in bomRequests)
            {
                r.ProductId = (long)entity.Id;

                var bom = _mapper.Map<Entities.Bom>(r);
                var bomLines = r.BomDetails ?? new List<BomDetailRequest>();

                bom.ProductId = (long)entity.Id;
                await _boms.Add(bom, token);
                await _bomDetails.AddRange(bomLines.Select(line => ToLineEntity(line, bom.Id)).ToList(), token);
            }

            // 2. Map + insert routing
            foreach (var r in routingRequests)
            {
                r.ProductId = (long)entity.Id;

                var routing = _mapper.Map<Entities.Routing>(r);
                var routingLines = r.RoutingOperations ?? new List<RoutingOperationRequest>();

                routing.ProductId = (long)entity.Id;
                await _routing.Add(routing, token);
                await _routingOp.AddRange(routingLines.Select(line => ToLineEntity(line, routing.Id)).ToList(), token);
            }

            return Result<ProductDto>.Success(_mapper.Map<ProductDto>(entity));
        }, ct);
    }

    public async Task<Result<ProductDto>> UpdateAsync(ulong id, ProductRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);

        if (entity is null)
        {
            return Result<ProductDto>.Failure($"Product {id} was not found.");
        }

        // Check duplicate ProductCode
        var existing = await _repository.FirstOrDefault(x => x.Id != id && x.ProductCode == request.ProductCode, ct);

        if (existing is not null)
        {
            return Result<ProductDto>.Failure($"Product '{request.ProductCode}' already exists.");
        }

        // Load existing BOMs
        var storedBoms = await _boms.Where(x => x.ProductId == (long)id, ct);

        var bomRequests = request.Boms;

        var keptBomIds = (bomRequests ?? new List<BomRequest>())
            .Where(x => x.Id != 0)
            .Select(x => x.Id)
            .ToHashSet();

        // Reject BOMs that don't belong to this Product
        var foreignBomIds = keptBomIds
            .Where(bomId => storedBoms.All(x => x.Id != bomId))
            .ToList();

        if (foreignBomIds.Count > 0)
        {
            return Result<ProductDto>.Failure($"BOM(s) {string.Join(", ", foreignBomIds)} do not belong to Product {id}.");
        }

        // Load existing Routings
        var storedRoutings = await _routing.Where(x => x.ProductId == (long)id, ct);

        var routingRequests = request.Routings;

        var keptRoutingIds = (routingRequests ?? new List<RoutingRequest>())
            .Where(x => x.Id != 0)
            .Select(x => x.Id)
            .ToHashSet();

        // Reject BOMs that don't belong to this Product
        var foreignRoutingIds = keptRoutingIds
            .Where(rouId => storedRoutings.All(x => x.Id != rouId))
            .ToList();

        if (foreignRoutingIds.Count > 0)
        {
            return Result<ProductDto>.Failure($"Routing(s) {string.Join(", ", foreignRoutingIds)} do not belong to Product {id}.");
        }

        _mapper.Map(request, entity);

        return await _unitOfWork.ExecuteAsync(async token =>
        {

            await _repository.Update(entity, token);

            //Process boms
            if (bomRequests is not null)
            {
                // =========================
                // 2. Delete removed BOMs
                // =========================
                var bomRemoves = storedBoms
                    .Where(x => !keptBomIds.Contains(x.Id))
                    .ToList();

                foreach (var bom in bomRemoves)
                {
                    var details = await _bomDetails.Where(
                        x => x.BomId == (long)bom.Id,
                        token);

                    await _bomDetails.DeleteRange(details, token);
                }

                await _boms.DeleteRange(bomRemoves, token);

                // =========================
                // 3. Update / Add BOM
                // =========================
                foreach (var bomRequest in bomRequests)
                {
                    Entities.Bom bom;

                    if (bomRequest.Id != 0)
                    {
                        // -------------------------
                        // Update existing BOM
                        // -------------------------
                        bom = storedBoms.First(x => x.Id == bomRequest.Id);

                        _mapper.Map(bomRequest, bom);

                        // Đảm bảo BOM vẫn thuộc Product hiện tại
                        bom.ProductId = (long)id;

                        await _boms.Update(bom, token);
                    }
                    else
                    {
                        // -------------------------
                        // Add new BOM
                        // -------------------------
                        bomRequest.ProductId = (long)id;

                        bom = _mapper.Map<Entities.Bom>(bomRequest);
                        bom.ProductId = (long)id;

                        await _boms.Add(bom, token);
                    }

                    // =========================
                    // 4. Update / Add BOM Details
                    // =========================
                    var details = bomRequest.BomDetails;

                    if (details is null)
                        continue;

                    var storedDetails = await _bomDetails.Where(
                        x => x.BomId == (long)bom.Id,
                        token);

                    var keptDetailIds = details
                        .Where(x => x.Id != 0)
                        .Select(x => x.Id)
                        .ToHashSet();

                    // Delete removed details
                    var detailRemoves = storedDetails
                        .Where(x => !keptDetailIds.Contains(x.Id))
                        .ToList();

                    await _bomDetails.DeleteRange(detailRemoves, token);

                    // Update existing details
                    foreach (var detailRequest in details.Where(x => x.Id != 0))
                    {
                        var detail = storedDetails
                            .First(x => x.Id == detailRequest.Id);

                        _mapper.Map(detailRequest, detail);

                        // Không cho phép đổi BOM
                        detail.BomId = (long)bom.Id;

                        await _bomDetails.Update(detail, token);
                    }

                    // Add new details
                    var detailAdds = details
                        .Where(x => x.Id == 0)
                        .Select(x => ToLineEntity(x, bom.Id))
                        .ToList();

                    await _bomDetails.AddRange(detailAdds, token);
                }
            }

            //Process routing
            if (routingRequests is not null)
            {
                // =========================
                // 2. Delete removed Routings
                // =========================
                var routingRemoves = storedRoutings
                    .Where(x => !keptRoutingIds.Contains(x.Id))
                    .ToList();

                foreach (var rou in routingRemoves)
                {
                    var details = await _routingOp.Where(
                        x => x.RoutingId == (long)rou.Id,
                        token);

                    await _routingOp.DeleteRange(details, token);
                }

                await _routing.DeleteRange(routingRemoves, token);

                // =========================
                // 3. Update / Add Routing
                // =========================
                foreach (var request in routingRequests)
                {
                    Entities.Routing routing;

                    if (request.Id != 0)
                    {
                        // -------------------------
                        // Update existing BOM
                        // -------------------------
                        routing = storedRoutings.First(x => x.Id == request.Id);

                        _mapper.Map(request, routing);

                        // Đảm bảo BOM vẫn thuộc Product hiện tại
                        routing.ProductId = (long)id;

                        await _routing.Update(routing, token);
                    }
                    else
                    {
                        // -------------------------
                        // Add new BOM
                        // -------------------------
                        request.ProductId = (long)id;

                        routing = _mapper.Map<Entities.Routing>(request);
                        routing.ProductId = (long)id;

                        await _routing.Add(routing, token);
                    }

                    // =========================
                    // 4. Update / Add BOM Details
                    // =========================
                    var details = request.RoutingOperations;

                    if (details is null) continue;

                    var storedDetails = await _routingOp.Where(x => x.RoutingId == (long)routing.Id, token);

                    var keptDetailIds = details.Where(x => x.Id != 0)
                                                .Select(x => x.Id)
                                                .ToHashSet();

                    // Delete removed details
                    var detailRemoves = storedDetails
                        .Where(x => !keptDetailIds.Contains(x.Id))
                        .ToList();

                    await _routingOp.DeleteRange(detailRemoves, token);

                    // Update existing details
                    foreach (var detailRequest in details.Where(x => x.Id != 0))
                    {
                        var detail = storedDetails.First(x => x.Id == detailRequest.Id);

                        _mapper.Map(detailRequest, detail);

                        // Không cho phép đổi BOM
                        detail.RoutingId = (long)routing.Id;

                        await _routingOp.Update(detail, token);
                    }

                    // Add new details
                    var detailAdds = details
                        .Where(x => x.Id == 0)
                        .Select(x => ToLineEntity(x, routing.Id))
                        .ToList();

                    await _routingOp.AddRange(detailAdds, token);
                }
            }

            return Result<ProductDto>.Success(_mapper.Map<ProductDto>(entity));

        }, ct);
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


    private Entities.BomDetail ToLineEntity(BomDetailRequest line, ulong id)
    {
        var entity = _mapper.Map<Entities.BomDetail>(line);
        entity.BomId = (long)id;
        return entity;
    }

    private Entities.RoutingOperation ToLineEntity(RoutingOperationRequest line, ulong id)
    {
        var entity = _mapper.Map<Entities.RoutingOperation>(line);
        entity.RoutingId = (long)id;
        return entity;
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

    public async Task<Result<BomDto>> CreateAsync(BomRequest request, CancellationToken ct = default)
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

    public async Task<Result<BomDto>> UpdateAsync(ulong id, BomRequest request, CancellationToken ct = default)
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

    public async Task<Result<BomDetailDto>> CreateAsync(BomDetailRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.BomDetail>(request);
        await _repository.Add(entity, ct);
        return Result<BomDetailDto>.Success(_mapper.Map<BomDetailDto>(entity));
    }

    public async Task<Result<BomDetailDto>> UpdateAsync(ulong id, BomDetailRequest request, CancellationToken ct = default)
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

public class RoutingService : IRoutingService
{
    private readonly IRepository<Entities.Routing> _repository;
    private readonly IRepository<Entities.RoutingOperation> _operations;
    private readonly IMapper _mapper;

    public RoutingService(
        IRepository<Entities.Routing> repository,
        IRepository<Entities.RoutingOperation> operations,
        IMapper mapper)
    {
        _repository = repository;
        _operations = operations;
        _mapper = mapper;
    }

    public async Task<List<RoutingDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<RoutingDto>>(await _repository.GetAll(ct));

    public async Task<RoutingDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<RoutingDto>(entity);
    }

    public async Task<Result<RoutingDto>> CreateAsync(RoutingRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.Routing>(request);
        await _repository.Add(entity, ct);
        return Result<RoutingDto>.Success(_mapper.Map<RoutingDto>(entity));
    }

    public async Task<Result<RoutingDto>> UpdateAsync(ulong id, RoutingRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<RoutingDto>.Failure($"Routing {id} was not found.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<RoutingDto>.Success(_mapper.Map<RoutingDto>(entity));
    }

    /// <summary>Deletes the routing together with its operations — they exist only for it.</summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Routing {id} was not found.");
        }

        var routingId = (long)id;
        await _operations.DeleteRange(await _operations.Where(o => o.RoutingId == routingId, ct), ct);
        await _repository.Delete(entity, ct);
        return Result.Success();
    }
}

public class RoutingOperationService : IRoutingOperationService
{
    private readonly IRepository<Entities.RoutingOperation> _repository;
    private readonly IMapper _mapper;

    public RoutingOperationService(IRepository<Entities.RoutingOperation> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<RoutingOperationDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<RoutingOperationDto>>(await _repository.GetAll(ct));

    public async Task<RoutingOperationDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<RoutingOperationDto>(entity);
    }

    public async Task<Result<RoutingOperationDto>> CreateAsync(RoutingOperationRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.RoutingOperation>(request);
        await _repository.Add(entity, ct);
        return Result<RoutingOperationDto>.Success(_mapper.Map<RoutingOperationDto>(entity));
    }

    public async Task<Result<RoutingOperationDto>> UpdateAsync(ulong id, RoutingOperationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<RoutingOperationDto>.Failure($"Routing operation {id} was not found.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<RoutingOperationDto>.Success(_mapper.Map<RoutingOperationDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Routing operation {id} was not found.");
    }
}

