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
    [Route("api/master-data/failure-code")]
    [ApiController]
    [Authorize]
    public class FailureCodeController : ControllerBase
    {
        private readonly IFailureCodeService _service;
        public FailureCodeController(IFailureCodeService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.FailureCode.View)]
        public async Task<ActionResult<ApiResponse<List<FailureCodeDto>>>> GetAll(CancellationToken ct)
        {
            var failureCodes = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(failureCodes));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.FailureCode.View)]
        public async Task<ActionResult<ApiResponse<FailureCodeDto>>> GetById(ulong id, CancellationToken ct)
        {
            var failureCode = await _service.GetByIdAsync(id, ct);
            if (failureCode is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Failure Code {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(failureCode));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.FailureCode.Add)]
        public async Task<ActionResult<ApiResponse<FailureCodeDto>>> Create(FailureCodeRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.FailureCode.Edit)]
        public async Task<ActionResult<ApiResponse<FailureCodeDto>>> Update(ulong id, FailureCodeRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.FailureCode.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Failure Code deleted."));
        }
    }
}
