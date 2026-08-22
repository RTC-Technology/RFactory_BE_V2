using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Application.Modules.PurchaseOrder.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.PurchaseOrder
{
    [Route("api/purchase-order/delivery-schedules")]
    [ApiController]
    [Authorize]
    public class PurchaseOrderDeliveryScheduleController : ControllerBase
    {
        private readonly IPurchaseOrderDeliveryScheduleService _service;

        public PurchaseOrderDeliveryScheduleController(IPurchaseOrderDeliveryScheduleService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PurchaseOrderDeliverySchedule.View)]
        public async Task<ActionResult<ApiResponse<List<PurchaseOrderDeliveryScheduleDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PurchaseOrderDeliverySchedule.View)]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDeliveryScheduleDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Purchase order delivery schedule line {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PurchaseOrderDeliverySchedule.Add)]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDeliveryScheduleDto>>> Create(PurchaseOrderDeliveryScheduleRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PurchaseOrderDeliverySchedule.Edit)]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDeliveryScheduleDto>>> Update(ulong id, PurchaseOrderDeliveryScheduleRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PurchaseOrderDeliverySchedule.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Purchase order delivery schedule line deleted."));
        }
    }
}
