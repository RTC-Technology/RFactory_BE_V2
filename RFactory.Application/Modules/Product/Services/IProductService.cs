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

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<Result<ProductDto>> UpdateAsync(ulong id, UpdateProductRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IBomService
{
    Task<List<BomDto>> GetAllAsync(CancellationToken ct = default);
    Task<BomDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<BomDto>> CreateAsync(CreateBomRequest request, CancellationToken ct = default);
    Task<Result<BomDto>> UpdateAsync(ulong id, UpdateBomRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IBomDetailService
{
    Task<List<BomDetailDto>> GetAllAsync(CancellationToken ct = default);
    Task<BomDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<BomDetailDto>> CreateAsync(CreateBomDetailRequest request, CancellationToken ct = default);
    Task<Result<BomDetailDto>> UpdateAsync(ulong id, UpdateBomDetailRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IRoutingService
{
    Task<List<RoutingDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoutingDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<RoutingDto>> CreateAsync(CreateRoutingRequest request, CancellationToken ct = default);
    Task<Result<RoutingDto>> UpdateAsync(ulong id, UpdateRoutingRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IRoutingOperationService
{
    Task<List<RoutingOperationDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoutingOperationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<RoutingOperationDto>> CreateAsync(CreateRoutingOperationRequest request, CancellationToken ct = default);
    Task<Result<RoutingOperationDto>> UpdateAsync(ulong id, UpdateRoutingOperationRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

