using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Application.Modules.Organizations.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Organizations
{
    [Route("api/organization/production-team/employees")]
    [ApiController]
    [Authorize]

    public class ProductionTeamEmployeeController : ControllerBase
    {
        private readonly IProductionTeamEmployeeService _service;

        public ProductionTeamEmployeeController(IProductionTeamEmployeeService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.ProductionTeamEmployee.View)]
        public async Task<ActionResult<ApiResponse<List<ProductionTeamEmployeeDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.ProductionTeamEmployee.View)]
        public async Task<ActionResult<ApiResponse<ProductionTeamEmployeeDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Production team employee {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.ProductionTeamEmployee.Add)]
        public async Task<ActionResult<ApiResponse<ProductionTeamEmployeeDto>>> Create(ProductionTeamEmployeeRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.ProductionTeamEmployee.Edit)]
        public async Task<ActionResult<ApiResponse<ProductionTeamEmployeeDto>>> Update(ulong id, ProductionTeamEmployeeRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.ProductionTeamEmployee.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Production team employee deleted."));
        }
    }
}
