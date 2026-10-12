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
    [Route("api/master-data/traceability-rule")]
    [ApiController]
    [Authorize]

    public class TraceabilityRuleController : ControllerBase
    {
        private readonly ITraceabilityRuleService _service;

        public TraceabilityRuleController(ITraceabilityRuleService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.TraceabilityRule.View)]
        public async Task<ActionResult<ApiResponse<List<TraceabilityRuleDto>>>> GetAll(CancellationToken ct)
        {
            var traceabilityRules = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(traceabilityRules));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityRule.View)]
        public async Task<ActionResult<ApiResponse<TraceabilityRuleDto>>> GetById(ulong id, CancellationToken ct)
        {
            var traceabilityRule = await _service.GetByIdAsync(id, ct);
            if (traceabilityRule is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Traceability Rule {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(traceabilityRule));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.TraceabilityRule.Add)]
        public async Task<ActionResult<ApiResponse<TraceabilityRuleDto>>> Create(TraceabilityRuleRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityRule.Edit)]
        public async Task<ActionResult<ApiResponse<TraceabilityRuleDto>>> Update(ulong id, TraceabilityRuleRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityRule.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Traceability Rule deleted."));
        }
    }
}
