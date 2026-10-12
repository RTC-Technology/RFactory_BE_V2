using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Application.Modules.PickingPlan.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.PickingPlan
{
    [Route("api/picking-plan/sources")]
    [ApiController]
    [Authorize]
    public class PickingPlanSourceController : ControllerBase
    {
        private readonly IPickingPlanSourceService _service;

        public PickingPlanSourceController(IPickingPlanSourceService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PickingPlanSource.View)]
        public async Task<ActionResult<ApiResponse<List<PickingPlanSourceDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanSource.View)]
        public async Task<ActionResult<ApiResponse<PickingPlanSourceDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Picking plan source {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PickingPlanSource.Add)]
        public async Task<ActionResult<ApiResponse<PickingPlanSourceDto>>> Create(PickingPlanSourceRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanSource.Edit)]
        public async Task<ActionResult<ApiResponse<PickingPlanSourceDto>>> Update(ulong id, PickingPlanSourceRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanSource.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Picking plan source deleted."));
        }
    }
}
