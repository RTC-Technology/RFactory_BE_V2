using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Application.Modules.Inventory.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Inventory
{
    [Route("api/inventory/transactions")]
    [ApiController]
    [Authorize]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionService _service;

        public InventoryTransactionController(IInventoryTransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.InventoryTransaction.View)]
        public async Task<ActionResult<ApiResponse<List<InventoryTransactionDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.InventoryTransaction.View)]
        public async Task<ActionResult<ApiResponse<InventoryTransactionDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Inventory transaction {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.InventoryTransaction.Add)]
        public async Task<ActionResult<ApiResponse<InventoryTransactionDto>>> Create(CreateInventoryTransactionRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.InventoryTransaction.Edit)]
        public async Task<ActionResult<ApiResponse<InventoryTransactionDto>>> Update(ulong id, UpdateInventoryTransactionRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.InventoryTransaction.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Inventory transaction deleted."));
        }
    }
}
