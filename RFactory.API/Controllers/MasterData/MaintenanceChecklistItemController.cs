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
    [Route("api/master-data/maintenance-checklist/items")]
    [ApiController]
    [Authorize]
    public class MaintenanceChecklistItemController : ControllerBase
    {
        private readonly IMaintenanceChecklistItemService _service;
        public MaintenanceChecklistItemController(IMaintenanceChecklistItemService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.MaintenanceChecklistItem.View)]
        public async Task<ActionResult<ApiResponse<List<MaintenanceChecklistItemDto>>>> GetAll(CancellationToken ct)
        {
            var maintenanceChecklistItems = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(maintenanceChecklistItems));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceChecklistItem.View)]
        public async Task<ActionResult<ApiResponse<MaintenanceChecklistItemDto>>> GetById(ulong id, CancellationToken ct)
        {
            var maintenanceChecklistItem = await _service.GetByIdAsync(id, ct);
            if (maintenanceChecklistItem is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Maintenance Checklist Item {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(maintenanceChecklistItem));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.MaintenanceChecklistItem.Add)]
        public async Task<ActionResult<ApiResponse<MaintenanceChecklistItemDto>>> Create(MaintenanceChecklistItemRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceChecklistItem.Edit)]
        public async Task<ActionResult<ApiResponse<MaintenanceChecklistItemDto>>> Update(ulong id, MaintenanceChecklistItemRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceChecklistItem.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Maintenance Checklist Item deleted."));
        }
    }
}
