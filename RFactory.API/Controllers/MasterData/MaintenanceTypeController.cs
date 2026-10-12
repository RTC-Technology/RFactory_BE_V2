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
    [Route("api/master-data/maintenance-type")]
    [ApiController]
    [Authorize]

    public class MaintenanceTypeController : ControllerBase
    {
        private readonly IMaintenanceTypeService _service;

        public MaintenanceTypeController(IMaintenanceTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.MaintenanceType.View)]
        public async Task<ActionResult<ApiResponse<List<MaintenanceTypeDto>>>> GetAll(CancellationToken ct)
        {
            var maintenanceTypes = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(maintenanceTypes));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceType.View)]
        public async Task<ActionResult<ApiResponse<MaintenanceTypeDto>>> GetById(ulong id, CancellationToken ct)
        {
            var maintenanceType = await _service.GetByIdAsync(id, ct);
            if (maintenanceType is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Maintenance Type {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(maintenanceType));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.MaintenanceType.Add)]
        public async Task<ActionResult<ApiResponse<MaintenanceTypeDto>>> Create(MaintenanceTypeRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceType.Edit)]
        public async Task<ActionResult<ApiResponse<MaintenanceTypeDto>>> Update(ulong id, MaintenanceTypeRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.MaintenanceType.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Maintenance Type deleted."));
        }
    }
}
