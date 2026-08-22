using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Application.Modules.PurchaseOrder.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.PurchaseOrder
{
    [Route("api/purchase-order/details")]
    [ApiController]
    public class PurchaseOrderDetailController : ControllerBase
    {
        private readonly IPurchaseOrderDetailService _service;

        public PurchaseOrderDetailController(IPurchaseOrderDetailService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PurchaseOrderDetail.View)]
        public async Task<ActionResult<ApiResponse<List<PurchaseOrderDetailDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PurchaseOrderDetail.View)]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDetailDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Purchase order line {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PurchaseOrderDetail.Add)]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDetailDto>>> Create(PurchaseOrderDetailRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PurchaseOrderDetail.Edit)]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDetailDto>>> Update(ulong id, PurchaseOrderDetailRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PurchaseOrderDetail.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Purchase order line deleted."));
        }
    }
}
