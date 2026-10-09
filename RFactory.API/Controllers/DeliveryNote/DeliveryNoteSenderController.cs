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
    [Route("api/delivery-note/senders")]
    [ApiController]
    [Authorize]
    public class DeliveryNoteSenderController : ControllerBase
    {
        private readonly IDeliveryNoteSenderService _service;

        public DeliveryNoteSenderController(IDeliveryNoteSenderService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.DeliveryNoteSender.View)]
        public async Task<ActionResult<ApiResponse<List<DeliveryNoteSenderDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.DeliveryNoteSender.View)]
        public async Task<ActionResult<ApiResponse<DeliveryNoteSenderDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Delivery note sender {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.DeliveryNoteSender.Add)]
        public async Task<ActionResult<ApiResponse<DeliveryNoteSenderDto>>> Create(DeliveryNoteSenderRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.DeliveryNoteSender.Edit)]
        public async Task<ActionResult<ApiResponse<DeliveryNoteSenderDto>>> Update(ulong id, DeliveryNoteSenderRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.DeliveryNoteSender.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Delivery note sender deleted."));
        }
    }
}
