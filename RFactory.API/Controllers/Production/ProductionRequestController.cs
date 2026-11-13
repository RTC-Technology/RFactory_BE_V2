using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Application.Modules.Production.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Production
{
    [Route("api/production/request")]
    [ApiController]
    [Authorize]

    public class ProductionRequestController : ControllerBase
    {
        private readonly IProductionRequestService _service;

        public ProductionRequestController(IProductionRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.DetailedProductionPlan.ProductionRequest.View)]
        public async Task<ActionResult<ApiResponse<List<ProductionRequestDto>>>> GetAll(CancellationToken ct)

        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.DetailedProductionPlan.ProductionRequest.View)]
        public async Task<ActionResult<ApiResponse<ProductionRequestDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Production request {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.DetailedProductionPlan.ProductionRequest.Add)]
        public async Task<ActionResult<ApiResponse<ProductionRequestDto>>> Create(ProductionRequestRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.DetailedProductionPlan.ProductionRequest.Edit)]
        public async Task<ActionResult<ApiResponse<ProductionRequestDto>>> Update(ulong id, ProductionRequestRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.DetailedProductionPlan.ProductionRequest.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Production request deleted."));
        }
    }
}
