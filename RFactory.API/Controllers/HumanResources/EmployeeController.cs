using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Application.Modules.HumanResources.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.HumanResources
{
    [Route("api/hr/employee")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Employee.View)]
        public async Task<ActionResult<ApiResponse<List<EmployeeDto>>>> GetAll(CancellationToken ct)
        {
            var employees = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(employees));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Employee.View)]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetById(ulong id, CancellationToken ct)
        {
            var employee = await _service.GetByIdAsync(id, ct);
            if (employee is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Employee {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(employee));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Employee.Add)]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create(CreateEmployeeRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Employee.Edit)]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(ulong id, UpdateEmployeeRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Employee.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Employee deleted."));
        }
    }
}