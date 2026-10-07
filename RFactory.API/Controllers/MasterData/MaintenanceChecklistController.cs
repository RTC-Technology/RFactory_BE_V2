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
    [Route("api/master-data/maintenance-checklist")]
    [ApiController]
    [Authorize]
    public class MaintenanceChecklistController : ControllerBase
    {
        private readonly IMaintenanceChecklistService _service;
        public MaintenanceChecklistController(IMaintenanceChecklistService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.MaintenanceChecklist.View)]
        public async Task<ActionResult<ApiResponse<List<MaintenanceChecklistDto>>>> GetAll(CancellationToken ct)
        {
            var maintenanceChecklists = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(maintenanceChecklists));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceChecklist.View)]
        public async Task<ActionResult<ApiResponse<MaintenanceChecklistDto>>> GetById(ulong id, CancellationToken ct)
        {
            var maintenanceChecklist = await _service.GetByIdAsync(id, ct);
            if (maintenanceChecklist is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Maintenance Checklist {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(maintenanceChecklist));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.MaintenanceChecklist.Add)]
        public async Task<ActionResult<ApiResponse<MaintenanceChecklistDto>>> Create(MaintenanceChecklistRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceChecklist.Edit)]
        public async Task<ActionResult<ApiResponse<MaintenanceChecklistDto>>> Update(ulong id, MaintenanceChecklistRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceChecklist.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Maintenance Checklist deleted."));
        }
    }
}
