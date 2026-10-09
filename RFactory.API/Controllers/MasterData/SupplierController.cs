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
    [Route("api/master-data/supplier")]
    [ApiController]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Supplier.View)]
        public async Task<ActionResult<ApiResponse<List<SupplierDto>>>> GetAll(CancellationToken ct)
        {
            var suppliers = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(suppliers));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Supplier.View)]
        public async Task<ActionResult<ApiResponse<SupplierDto>>> GetById(ulong id, CancellationToken ct)
        {
            var supplier = await _service.GetByIdAsync(id, ct);
            if (supplier is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Supplier {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(supplier));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Supplier.Add)]
        public async Task<ActionResult<ApiResponse<SupplierDto>>> Create(CreateSupplierRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Supplier.Edit)]
        public async Task<ActionResult<ApiResponse<SupplierDto>>> Update(ulong id, UpdateSupplierRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Supplier.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Suppler deleted."));
        }
    }
}
