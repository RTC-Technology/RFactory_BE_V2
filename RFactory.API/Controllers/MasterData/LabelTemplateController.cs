using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// Endpoints for managing customizable barcode &amp; QR label print templates.
/// </summary>
[ApiController]
[Route("api/label-templates")]
[Authorize]
public class LabelTemplateController : ControllerBase
{
    private readonly ILabelTemplateService _service;

    public LabelTemplateController(ILabelTemplateService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.LabelTemplate.View)]
    public async Task<ActionResult<ApiResponse<List<LabelTemplateDto>>>> GetAll(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.LabelTemplate.View)]
    public async Task<ActionResult<ApiResponse<LabelTemplateDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Failure<LabelTemplateDto>($"Template #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpGet("entity/{entityType}")]
    [RequirePermission(PermissionCodes.LabelTemplate.View)]
    public async Task<ActionResult<ApiResponse<List<LabelTemplateDto>>>> GetByEntityType(string entityType, CancellationToken ct)
    {
        var list = await _service.GetByEntityTypeAsync(entityType, ct);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("entity/{entityType}/default")]
    [RequirePermission(PermissionCodes.LabelTemplate.View)]
    public async Task<ActionResult<ApiResponse<LabelTemplateDto>>> GetDefaultByEntityType(string entityType, CancellationToken ct)
    {
        var item = await _service.GetDefaultByEntityTypeAsync(entityType, ct);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Failure<LabelTemplateDto>($"No default template found for entity type '{entityType}'", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.LabelTemplate.Add)]
    public async Task<ActionResult<ApiResponse<LabelTemplateDto>>> Create([FromBody] LabelTemplateRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<LabelTemplateDto>(result.Error, HttpStatusCode.BadRequest));
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.LabelTemplate.Edit)]
    public async Task<ActionResult<ApiResponse<LabelTemplateDto>>> Update(ulong id, [FromBody] LabelTemplateRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<LabelTemplateDto>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.LabelTemplate.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<bool>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(true));
    }

    [HttpPost("{id:long}/set-default")]
    [RequirePermission(PermissionCodes.LabelTemplate.Edit)]
    public async Task<ActionResult<ApiResponse<bool>>> SetDefault(ulong id, CancellationToken ct)
    {
        var result = await _service.SetDefaultAsync(id, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<bool>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(true));
    }
}
