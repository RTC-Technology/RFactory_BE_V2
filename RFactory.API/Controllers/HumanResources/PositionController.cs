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
/// CRUD endpoints for job position master data.
/// </summary>
[ApiController]
[Route("api/hr/positions")]
[Authorize]
public class PositionController : ControllerBase
{
    private readonly IPositionService _service;

    public PositionController(IPositionService service) => _service = service;

    [HttpGet]
    [RequirePermission(PermissionCodes.Position.View)]
    public async Task<ActionResult<ApiResponse<List<PositionDto>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponseFactory.Success(await _service.GetAllAsync(ct)));

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Position.View)]
    public async Task<ActionResult<ApiResponse<PositionDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null
            ? NotFound(ApiResponseFactory.Fail($"Position {id} was not found.", HttpStatusCode.NotFound))
            : Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Position.Add)]
    public async Task<ActionResult<ApiResponse<PositionDto>>> Create(CreatePositionRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success(result.Data))
            : BadRequest(ApiResponseFactory.Fail(result.Error!));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Position.Edit)]
    public async Task<ActionResult<ApiResponse<PositionDto>>> Update(ulong id, UpdatePositionRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success(result.Data))
            : NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Position.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Succeeded
            ? Ok(ApiResponseFactory.Success<object?>(null, "Position deleted."))
            : NotFound(ApiResponseFactory.Fail(result.Error!, HttpStatusCode.NotFound));
    }
}
