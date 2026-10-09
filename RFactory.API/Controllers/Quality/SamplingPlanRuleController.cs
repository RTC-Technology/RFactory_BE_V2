using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Application.Modules.Quality.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Quality
{
    [Route("api/quality/sampling-plan/rules")]
    [ApiController]
    [Authorize]

    public class SamplingPlanRuleController : ControllerBase
    {
        private readonly ISamplingPlanRuleService _service;
        public SamplingPlanRuleController(ISamplingPlanRuleService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.SamplingPlanRule.View)]
        public async Task<ActionResult<ApiResponse<List<SamplingPlanRuleDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.SamplingPlanRule.View)]
        public async Task<ActionResult<ApiResponse<SamplingPlanRuleDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Sampling plan rule {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.SamplingPlanRule.Add)]
        public async Task<ActionResult<ApiResponse<SamplingPlanRuleDto>>> Create(SamplingPlanRuleRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.SamplingPlanRule.Edit)]
        public async Task<ActionResult<ApiResponse<SamplingPlanRuleDto>>> Update(ulong id, SamplingPlanRuleRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.SamplingPlanRule.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Sampling plan rule deleted."));
        }
    }
}
