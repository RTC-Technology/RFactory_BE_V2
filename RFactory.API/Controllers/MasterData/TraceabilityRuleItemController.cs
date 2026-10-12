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
    [Route("api/master-data/traceability-rule/items")]
    [ApiController]
    [Authorize]

    public class TraceabilityRuleItemController : ControllerBase
    {
        private readonly ITraceabilityRuleItemService _service;

        public TraceabilityRuleItemController(ITraceabilityRuleItemService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.TraceabilityRuleItem.View)]
        public async Task<ActionResult<ApiResponse<List<TraceabilityRuleItemDto>>>> GetAll(CancellationToken ct)
        {
            var traceabilityRules = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(traceabilityRules));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityRuleItem.View)]
        public async Task<ActionResult<ApiResponse<TraceabilityRuleItemDto>>> GetById(ulong id, CancellationToken ct)
        {
            var traceabilityRule = await _service.GetByIdAsync(id, ct);
            if (traceabilityRule is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Traceability Rule Item {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(traceabilityRule));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.TraceabilityRuleItem.Add)]
        public async Task<ActionResult<ApiResponse<TraceabilityRuleItemDto>>> Create(TraceabilityRuleItemRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityRuleItem.Edit)]
        public async Task<ActionResult<ApiResponse<TraceabilityRuleItemDto>>> Update(ulong id, TraceabilityRuleItemRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityRuleItem.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Traceability Rule Item deleted."));
        }
    }
}
