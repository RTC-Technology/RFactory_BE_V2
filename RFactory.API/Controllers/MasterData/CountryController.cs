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
    [Route("api/master-data/country")]
    [ApiController]
    [Authorize]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _service;
        public CountryController(ICountryService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Country.View)]
        public async Task<ActionResult<ApiResponse<List<CountryDto>>>> GetAll(CancellationToken ct)
        {
            var countries = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(countries));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Country.View)]
        public async Task<ActionResult<ApiResponse<CountryDto>>> GetById(ulong id, CancellationToken ct)
        {
            var country = await _service.GetByIdAsync(id, ct);
            if (country is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Country {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(country));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Country.Add)]
        public async Task<ActionResult<ApiResponse<CountryDto>>> Create(CountryRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Country.Edit)]
        public async Task<ActionResult<ApiResponse<CountryDto>>> Update(ulong id, CountryRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Country.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Country deleted."));
        }
    }
}
