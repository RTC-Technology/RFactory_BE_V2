using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.DeliveryNote.Services;
using RFactory.Application.Modules.Packing.DTOs;
using RFactory.Application.Modules.Packing.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Packing
{
    [Route("api/packing/check")]
    [ApiController]
    [Authorize]
    public class PackingCheckController : ControllerBase
    {
        private readonly IPackingCheckService _service;

        public PackingCheckController(IPackingCheckService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PackingCheck.View)]
        public async Task<ActionResult<ApiResponse<List<PackingCheckDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PackingCheck.View)]
        public async Task<ActionResult<ApiResponse<PackingCheckDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Packing check {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PackingCheck.Add)]
        public async Task<ActionResult<ApiResponse<PackingCheckDto>>> Create(PackingCheckRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PackingCheck.Edit)]
        public async Task<ActionResult<ApiResponse<PackingCheckDto>>> Update(ulong id, PackingCheckRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PackingCheck.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Packing check deleted."));
        }
    }
}
