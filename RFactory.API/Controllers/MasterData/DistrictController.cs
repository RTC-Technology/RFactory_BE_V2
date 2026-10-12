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
    [Route("api/master-data/district")]
    [ApiController]
    [Authorize]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _service;
        public DistrictController(IDistrictService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.District.View)]
        public async Task<ActionResult<ApiResponse<List<DistrictDto>>>> GetAll(CancellationToken ct)
        {
            var districts = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(districts));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.District.View)]
        public async Task<ActionResult<ApiResponse<DistrictDto>>> GetById(ulong id, CancellationToken ct)
        {
            var district = await _service.GetByIdAsync(id, ct);
            if (district is null)
            {
                return NotFound(ApiResponseFactory.Fail($"District {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(district));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.District.Add)]
        public async Task<ActionResult<ApiResponse<DistrictDto>>> Create(DistrictRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.District.Edit)]
        public async Task<ActionResult<ApiResponse<DistrictDto>>> Update(ulong id, DistrictRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.District.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "District deleted."));
        }
    }
}
