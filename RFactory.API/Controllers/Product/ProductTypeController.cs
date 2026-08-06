using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product;

/// <summary>
/// CRUD endpoints for Product type.
/// </summary>
[ApiController]
[Route("api/product/product-types")]
[Authorize]
public class ProductTypeController : ControllerBase
{
    private readonly IProductTypeService _productTypeService;

    public ProductTypeController(IProductTypeService service)
    {
        _productTypeService = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.ProductType.View)]
    public async Task<ActionResult<ApiResponse<List<ProductTypeDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _productTypeService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.ProductType.View)]
    public async Task<ActionResult<ApiResponse<ProductTypeDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _productTypeService.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Product type {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.ProductType.Add)]
    public async Task<ActionResult<ApiResponse<ProductTypeDto>>> Create(CreateProductTypeRequest request, CancellationToken ct)
    {
        var result = await _productTypeService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.ProductType.Edit)]
    public async Task<ActionResult<ApiResponse<ProductTypeDto>>> Update(ulong id, UpdateProductTypeRequest request, CancellationToken ct)
    {
        var result = await _productTypeService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.ProductType.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _productTypeService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Product type deleted."));
    }
}