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
    [Route("api/picking-plan/item-sources")]
    [ApiController]
    [Authorize]
    public class PickingPlanItemSourceController : ControllerBase
    {
        private readonly IPickingPlanItemSourceService _service;

        public PickingPlanItemSourceController(IPickingPlanItemSourceService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PickingPlanItemSource.View)]
        public async Task<ActionResult<ApiResponse<List<PickingPlanItemSourceDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanItemSource.View)]
        public async Task<ActionResult<ApiResponse<PickingPlanItemSourceDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Picking plan item source {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PickingPlanItemSource.Add)]
        public async Task<ActionResult<ApiResponse<PickingPlanItemSourceDto>>> Create(PickingPlanItemSourceRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanItemSource.Edit)]
        public async Task<ActionResult<ApiResponse<PickingPlanItemSourceDto>>> Update(ulong id, PickingPlanItemSourceRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanItemSource.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Picking plan item source deleted."));
        }
    }
}
