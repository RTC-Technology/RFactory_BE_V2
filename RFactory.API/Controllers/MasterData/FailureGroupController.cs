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
    [Route("api/master-data/failure-group")]
    [ApiController]
    [Authorize]

    public class FailureGroupController : ControllerBase
    {
        private readonly IFailureGroupService _service;
        public FailureGroupController(IFailureGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.FailureGroup.View)]
        public async Task<ActionResult<ApiResponse<List<FailureGroupDto>>>> GetAll(CancellationToken ct)
        {
            var failureGroups = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(failureGroups));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.FailureGroup.View)]
        public async Task<ActionResult<ApiResponse<FailureGroupDto>>> GetById(ulong id, CancellationToken ct)
        {
            var failureGroup = await _service.GetByIdAsync(id, ct);
            if (failureGroup is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Failure Group {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(failureGroup));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.FailureGroup.Add)]
        public async Task<ActionResult<ApiResponse<FailureGroupDto>>> Create(FailureGroupRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.FailureGroup.Edit)]
        public async Task<ActionResult<ApiResponse<FailureGroupDto>>> Update(ulong id, FailureGroupRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.FailureGroup.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Failure Group deleted."));
        }
    }
}
