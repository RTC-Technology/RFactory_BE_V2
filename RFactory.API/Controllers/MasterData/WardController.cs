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
    [Route("api/master-data/ward")]
    [ApiController]
    [Authorize]
    public class WardController : ControllerBase
    {
        private readonly IWardService _service;

        public WardController(IWardService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Ward.View)]
        public async Task<ActionResult<ApiResponse<List<WardDto>>>> GetAll(CancellationToken ct)
        {
            var wards = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(wards));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Ward.View)]
        public async Task<ActionResult<ApiResponse<WardDto>>> GetById(ulong id, CancellationToken ct)
        {
            var ward = await _service.GetByIdAsync(id, ct);
            if (ward is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Ward {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(ward));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Ward.Add)]
        public async Task<ActionResult<ApiResponse<WardDto>>> Create(WardRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Ward.Edit)]
        public async Task<ActionResult<ApiResponse<WardDto>>> Update(ulong id, WardRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Ward.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Ward deleted."));
        }
    }
}
