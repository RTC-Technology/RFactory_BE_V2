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
    [Route("api/production/sales-order/payment-schedules")]
    [ApiController]
    [Authorize]
    public class SalesOrderPaymentScheduleController : ControllerBase
    {
        private readonly ISalesOrderPaymentScheduleService _service;

        public SalesOrderPaymentScheduleController(ISalesOrderPaymentScheduleService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.SalesOrderPaymentSchedule.View)]
        public async Task<ActionResult<ApiResponse<List<SalesOrderPaymentScheduleDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.SalesOrderPaymentSchedule.View)]
        public async Task<ActionResult<ApiResponse<SalesOrderPaymentScheduleDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Sales order payment schedule {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.SalesOrderPaymentSchedule.Add)]
        public async Task<ActionResult<ApiResponse<SalesOrderPaymentScheduleDto>>> Create(SalesOrderPaymentScheduleRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.SalesOrderPaymentSchedule.Edit)]
        public async Task<ActionResult<ApiResponse<SalesOrderPaymentScheduleDto>>> Update(ulong id, SalesOrderPaymentScheduleRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.SalesOrderPaymentSchedule.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Sales order payment schedule deleted."));
        }
    }
}
