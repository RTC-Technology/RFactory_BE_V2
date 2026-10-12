using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Product
{
    [Route("api/product/groups")]
    [ApiController]
    [Authorize]

    public class ProductGroupController : ControllerBase
    {
        private readonly IProductGroupService _service;

        public ProductGroupController(IProductGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.ProductGroup.View)]
        public async Task<ActionResult<ApiResponse<List<ProductGroupDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.ProductGroup.View)]
        public async Task<ActionResult<ApiResponse<ProductGroupDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Product group {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.ProductGroup.Add)]
        public async Task<ActionResult<ApiResponse<ProductGroupDto>>> Create(ProductGroupRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.ProductGroup.Edit)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(ulong id, ProductGroupRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.ProductGroup.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Product group deleted."));
        }
    }
}
