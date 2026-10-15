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
/// CRUD endpoints for the skill catalogue.
/// </summary>
[ApiController]
[Route("api/hr/skills")]
[Authorize]
public class SkillController : ControllerBase
{
    private readonly ISkillService _service;

    public SkillController(ISkillService service) => _service = service;

    [HttpGet]
    [RequirePermission(PermissionCodes.Skill.View)]
    public async Task<ActionResult<ApiResponse<List<SkillDto>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponseFactory.Success(await _service.GetAllAsync(ct)));

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Skill.View)]
    public async Task<ActionResult<ApiResponse<SkillDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null
            ? NotFound(ApiResponseFactory.Fail($"Skill {id} was not found.", HttpStatusCode.NotFound))
            : Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Skill.Add)]
    public async Task<ActionResult<ApiResponse<SkillDto>>> Create(CreateSkillRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success(result.Data))
            : BadRequest(ApiResponseFactory.Fail(result.Error!));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Skill.Edit)]
    public async Task<ActionResult<ApiResponse<SkillDto>>> Update(ulong id, UpdateSkillRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success(result.Data))
            : NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Skill.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success<object?>(null, "Skill deleted."))
            : NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
    }
}
