using RFactory.Application.Modules.Product.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Product.Services;

public interface IProductTypeService
{
    Task<List<ProductTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<ProductTypeDto>> CreateAsync(CreateProductTypeRequest request, CancellationToken ct = default);
    Task<Result<ProductTypeDto>> UpdateAsync(ulong id, UpdateProductTypeRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IProductGroupService
{
    Task<List<ProductGroupDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<ProductGroupDto>> CreateAsync(ProductGroupRequest request, CancellationToken ct = default);
    Task<Result<ProductGroupDto>> UpdateAsync(ulong id, ProductGroupRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<ProductDto>> CreateAsync(ProductRequest request, CancellationToken ct = default);
    Task<Result<ProductDto>> UpdateAsync(ulong id, ProductRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IBomService
{
    Task<List<BomDto>> GetAllAsync(CancellationToken ct = default);
    Task<BomDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<BomDto>> CreateAsync(BomRequest request, CancellationToken ct = default);
    Task<Result<BomDto>> UpdateAsync(ulong id, BomRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IBomDetailService
{
    Task<List<BomDetailDto>> GetAllAsync(CancellationToken ct = default);
    Task<BomDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<BomDetailDto>> CreateAsync(BomDetailRequest request, CancellationToken ct = default);
    Task<Result<BomDetailDto>> UpdateAsync(ulong id, BomDetailRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IRoutingService
{
    Task<List<RoutingDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoutingDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<RoutingDto>> CreateAsync(RoutingRequest request, CancellationToken ct = default);
    Task<Result<RoutingDto>> UpdateAsync(ulong id, RoutingRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IRoutingOperationService
{
    Task<List<RoutingOperationDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoutingOperationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<RoutingOperationDto>> CreateAsync(RoutingOperationRequest request, CancellationToken ct = default);
    Task<Result<RoutingOperationDto>> UpdateAsync(ulong id, RoutingOperationRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

