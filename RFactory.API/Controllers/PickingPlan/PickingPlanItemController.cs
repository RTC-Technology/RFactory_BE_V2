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
    [Route("api/picking-plan/items")]
    [ApiController]
    [Authorize]
    public class PickingPlanItemController : ControllerBase
    {
        private readonly IPickingPlanItemService _service;

        public PickingPlanItemController(IPickingPlanItemService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PickingPlanItem.View)]
        public async Task<ActionResult<ApiResponse<List<PickingPlanItemDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanItem.View)]
        public async Task<ActionResult<ApiResponse<PickingPlanItemDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Picking plan item {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PickingPlanItem.Add)]
        public async Task<ActionResult<ApiResponse<PickingPlanItemDto>>> Create(PickingPlanItemRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanItem.Edit)]
        public async Task<ActionResult<ApiResponse<PickingPlanItemDto>>> Update(ulong id, PickingPlanItemRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PickingPlanItem.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Picking plan item deleted."));
        }
    }
}
