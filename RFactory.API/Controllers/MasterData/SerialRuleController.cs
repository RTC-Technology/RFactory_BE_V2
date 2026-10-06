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
    [Route("api/master-data/serial-rule")]
    [ApiController]
    [Authorize]

    public class SerialRuleController : ControllerBase
    {
        private readonly ISerialRuleService _service;

        public SerialRuleController(ISerialRuleService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.SerialRule.View)]
        public async Task<ActionResult<ApiResponse<List<SerialRuleDto>>>> GetAll(CancellationToken ct)
        {
            var serialRules = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(serialRules));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.SerialRule.View)]
        public async Task<ActionResult<ApiResponse<SerialRuleDto>>> GetById(ulong id, CancellationToken ct)
        {
            var serialRule = await _service.GetByIdAsync(id, ct);
            if (serialRule is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Serial Rule {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(serialRule));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.SerialRule.Add)]
        public async Task<ActionResult<ApiResponse<SerialRuleDto>>> Create(SerialRuleRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.SerialRule.Edit)]
        public async Task<ActionResult<ApiResponse<SerialRuleDto>>> Update(ulong id, SerialRuleRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.SerialRule.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Serial Rule deleted."));
        }
    }
}
