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
    [Route("api/packing/package")]
    [ApiController]
    [Authorize]
    public class PackingPackageController : ControllerBase
    {
        private readonly IPackingPackageService _service;

        public PackingPackageController(IPackingPackageService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.PackingPackage.View)]
        public async Task<ActionResult<ApiResponse<List<PackingPackageDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.PackingPackage.View)]
        public async Task<ActionResult<ApiResponse<PackingPackageDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Packing package {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.PackingPackage.Add)]
        public async Task<ActionResult<ApiResponse<PackingPackageDto>>> Create(PackingPackageRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.PackingPackage.Edit)]
        public async Task<ActionResult<ApiResponse<PackingPackageDto>>> Update(ulong id, PackingPackageRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.PackingPackage.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Packing package deleted."));
        }
    }
}
