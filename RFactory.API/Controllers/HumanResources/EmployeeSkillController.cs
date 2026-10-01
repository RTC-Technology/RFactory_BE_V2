using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Application.Modules.HumanResources.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.HumanResources;

/// <summary>
/// CRUD endpoints for employee-skill assignments.
/// </summary>
[ApiController]
[Route("api/hr/employee-skills")]
[Authorize]
public class EmployeeSkillController : ControllerBase
{
    private readonly IEmployeeSkillService _service;

    public EmployeeSkillController(IEmployeeSkillService service) => _service = service;

    /// <summary>
    /// Returns all employee-skill records, optionally filtered to one employee.
    /// </summary>
    [HttpGet]
    [RequirePermission(PermissionCodes.EmployeeSkill.View)]
    public async Task<ActionResult<ApiResponse<List<EmployeeSkillDto>>>> GetAll(
        [FromQuery] ulong? employeeId, CancellationToken ct)
    {
        var items = employeeId.HasValue
            ? await _service.GetByEmployeeAsync(employeeId.Value, ct)
            : await _service.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.EmployeeSkill.View)]
    public async Task<ActionResult<ApiResponse<EmployeeSkillDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null
            ? NotFound(ApiResponseFactory.Fail($"Employee skill {id} was not found.", HttpStatusCode.NotFound))
            : Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.EmployeeSkill.Add)]
    public async Task<ActionResult<ApiResponse<EmployeeSkillDto>>> Create(
        CreateEmployeeSkillRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success(result.Data))
            : BadRequest(ApiResponseFactory.Fail(result.Error!));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.EmployeeSkill.Edit)]
    public async Task<ActionResult<ApiResponse<EmployeeSkillDto>>> Update(
        ulong id, UpdateEmployeeSkillRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success(result.Data))
            : NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.EmployeeSkill.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success<object?>(null, "Employee skill deleted."))
            : NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
    }
}
