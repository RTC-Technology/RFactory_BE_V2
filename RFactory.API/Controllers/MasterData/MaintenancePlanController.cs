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
    [Route("api/master-data/maintenance-plan")]
    [ApiController]
    [Authorize]
    public class MaintenancePlanController : ControllerBase
    {
        private readonly IMaintenancePlanService _service;
        public MaintenancePlanController(IMaintenancePlanService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.MaintenancePlan.View)]
        public async Task<ActionResult<ApiResponse<List<MaintenancePlanDto>>>> GetAll(CancellationToken ct)
        {
            var maintenancePlans = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(maintenancePlans));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenancePlan.View)]
        public async Task<ActionResult<ApiResponse<MaintenancePlanDto>>> GetById(ulong id, CancellationToken ct)
        {
            var maintenancePlan = await _service.GetByIdAsync(id, ct);
            if (maintenancePlan is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Maintenance Plan {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(maintenancePlan));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.MaintenancePlan.Add)]
        public async Task<ActionResult<ApiResponse<MaintenancePlanDto>>> Create(MaintenancePlanRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenancePlan.Edit)]
        public async Task<ActionResult<ApiResponse<MaintenancePlanDto>>> Update(ulong id, MaintenancePlanRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenancePlan.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Maintenance Plan deleted."));
        }
    }
}
