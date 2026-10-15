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
    [Route("api/production/sales-order/items")]
    [ApiController]
    [Authorize]

    public class SalesOrderItemController : ControllerBase
    {
        private readonly ISalesOrderItemService _service;

        public SalesOrderItemController(ISalesOrderItemService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.SalesOrderItem.View)]
        public async Task<ActionResult<ApiResponse<List<SalesOrderItemDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.SalesOrderItem.View)]
        public async Task<ActionResult<ApiResponse<SalesOrderItemDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Sales order item {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.SalesOrderItem.Add)]
        public async Task<ActionResult<ApiResponse<SalesOrderItemDto>>> Create(SalesOrderItemRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.SalesOrderItem.Edit)]
        public async Task<ActionResult<ApiResponse<SalesOrderItemDto>>> Update(ulong id, SalesOrderItemRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.SalesOrderItem.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Sales order item deleted."));
        }
    }
}
