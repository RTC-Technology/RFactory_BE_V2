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
    [Route("api/lot")]
    [ApiController]
    [Authorize]

    public class LotController : ControllerBase
    {
        private readonly ILotService _service;

        public LotController(ILotService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Lot.View)]
        public async Task<ActionResult<ApiResponse<List<LotDto>>>> GetAll(CancellationToken ct)
        {
            var lots = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(lots));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Lot.View)]
        public async Task<ActionResult<ApiResponse<LotDto>>> GetById(ulong id, CancellationToken ct)
        {
            var lot = await _service.GetByIdAsync(id, ct);
            if (lot is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Lot {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(lot));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Lot.Add)]
        public async Task<ActionResult<ApiResponse<LotDto>>> Create(LotRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Lot.Edit)]
        public async Task<ActionResult<ApiResponse<LotDto>>> Update(ulong id, LotRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Lot.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Lot deleted."));
        }
    }
}
