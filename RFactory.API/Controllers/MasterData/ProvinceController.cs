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
    [Route("api/master-data/province")]
    [ApiController]
    [Authorize]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _service;

        public ProvinceController(IProvinceService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Province.View)]
        public async Task<ActionResult<ApiResponse<List<ProvinceDto>>>> GetAll(CancellationToken ct)
        {
            var provinces = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(provinces));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Province.View)]
        public async Task<ActionResult<ApiResponse<ProvinceDto>>> GetById(ulong id, CancellationToken ct)
        {
            var province = await _service.GetByIdAsync(id, ct);
            if (province is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Province {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(province));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Province.Add)]
        public async Task<ActionResult<ApiResponse<ProvinceDto>>> Create(ProvinceRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Province.Edit)]
        public async Task<ActionResult<ApiResponse<ProvinceDto>>> Update(ulong id, ProvinceRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Province.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Province deleted."));
        }
    }
}
