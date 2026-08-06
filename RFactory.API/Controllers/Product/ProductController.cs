using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product;

/// <summary>
/// CRUD endpoints for Product.
/// </summary>
[ApiController]
[Route("api/product/products")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService service)
    {
        _productService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Product.View)]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _productService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Product.View)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _productService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Product {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Product.Add)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var result = await _productService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Product.Edit)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(ulong id, UpdateProductRequest request, CancellationToken ct)
    {
        var result = await _productService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Product.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _productService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Product deleted."));
    }
}