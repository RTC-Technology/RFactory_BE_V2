using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Packing.DTOs;
using RFactory.Application.Modules.Packing.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Packing
{
    [Route("api/packing/scan-logs")]
    [ApiController]
    [Authorize]
    public class PackingScanLogController : ControllerBase
    {
        private readonly IPackingScanLogService _service;

        public PackingScanLogController(IPackingScanLogService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PackingScanLog.View)]
        public async Task<ActionResult<ApiResponse<List<PackingScanLogDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PackingScanLog.View)]
        public async Task<ActionResult<ApiResponse<PackingScanLogDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Packing scan log {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PackingScanLog.Add)]
        public async Task<ActionResult<ApiResponse<PackingScanLogDto>>> Create(PackingScanLogRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PackingScanLog.Edit)]
        public async Task<ActionResult<ApiResponse<PackingScanLogDto>>> Update(ulong id, PackingScanLogRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PackingScanLog.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Packing scan log deleted."));
        }
    }
}
