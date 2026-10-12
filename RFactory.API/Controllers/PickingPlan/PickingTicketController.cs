using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Application.Modules.PickingPlan.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.PickingPlan
{
    [Route("api/picking-ticket")]
    [ApiController]
    [Authorize]
    public class PickingTicketController : ControllerBase
    {
        private readonly IPickingTicketService _service;

        public PickingTicketController(IPickingTicketService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PickingTicket.View)]
        public async Task<ActionResult<ApiResponse<List<PickingTicketDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PickingTicket.View)]
        public async Task<ActionResult<ApiResponse<PickingTicketDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Picking ticket {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PickingTicket.Add)]
        public async Task<ActionResult<ApiResponse<PickingTicketDto>>> Create(PickingTicketRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PickingTicket.Edit)]
        public async Task<ActionResult<ApiResponse<PickingTicketDto>>> Update(ulong id, PickingTicketRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PickingTicket.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Picking ticket deleted."));
        }
    }
}
