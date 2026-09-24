using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.DeliveryNote.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.DeliveryNote
{
    [Route("api/delivery-note/sources")]
    [ApiController]
    [Authorize]
    public class DeliveryNoteSourceController : ControllerBase
    {
        private readonly IDeliveryNoteSourceService _service;

        public DeliveryNoteSourceController(IDeliveryNoteSourceService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.DeliveryNoteSource.View)]
        public async Task<ActionResult<ApiResponse<List<DeliveryNoteSourceDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.DeliveryNoteSource.View)]
        public async Task<ActionResult<ApiResponse<DeliveryNoteSourceDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Delivery note source {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.DeliveryNoteSource.Add)]
        public async Task<ActionResult<ApiResponse<DeliveryNoteSourceDto>>> Create(DeliveryNoteSourceRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.DeliveryNoteSource.Edit)]
        public async Task<ActionResult<ApiResponse<DeliveryNoteSourceDto>>> Update(ulong id, DeliveryNoteSourceRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.DeliveryNoteSource.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Delivery note source deleted."));
        }
    }
}
