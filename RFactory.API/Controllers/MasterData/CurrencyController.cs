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
    [Route("api/master-data/currency")]
    [ApiController]
    [Authorize]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _service;
        public CurrencyController(ICurrencyService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Currency.View)]
        public async Task<ActionResult<ApiResponse<List<CurrencyDto>>>> GetAll(CancellationToken ct)
        {
            var currencies = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(currencies));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Currency.View)]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> GetById(ulong id, CancellationToken ct)
        {
            var currency = await _service.GetByIdAsync(id, ct);
            if (currency is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Currency {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(currency));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Currency.Add)]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> Create(CurrencyRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Currency.Edit)]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> Update(ulong id, CurrencyRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Currency.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Currency deleted."));
        }
    }
}
