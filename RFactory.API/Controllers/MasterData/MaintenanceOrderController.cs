using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData
{
    [Route("api/maintenance-order")]
    [ApiController]
    [Authorize]
    public class MaintenanceOrderController : ControllerBase
    {
        private readonly IMaintenanceOrderService _service;

        public MaintenanceOrderController(IMaintenanceOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.MaintenanceOrder.View)]
        public async Task<ActionResult<ApiResponse<List<MaintenanceOrderDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceOrder.View)]
        public async Task<ActionResult<ApiResponse<MaintenanceOrderDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Maintenance order {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.MaintenanceOrder.Add)]
        public async Task<ActionResult<ApiResponse<MaintenanceOrderDto>>> Create(MaintenanceOrderRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _service.CreateAsync(request, ct);
                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponseFactory.Fail(result.Error!));
                }

                return Ok(ApiResponseFactory.Success(result.Data));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceOrder.Edit)]
        public async Task<ActionResult<ApiResponse<MaintenanceOrderDto>>> Update(ulong id, MaintenanceOrderRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceOrder.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Maintenance order deleted."));
        }
    }
}
