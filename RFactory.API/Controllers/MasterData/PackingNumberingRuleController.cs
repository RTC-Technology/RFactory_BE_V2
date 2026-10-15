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
/// Endpoints for managing packaging numbering rules and automated code generation.
/// </summary>
[ApiController]
[Route("api/packing-numbering-rules")]
[Authorize]
public class PackingNumberingRuleController : ControllerBase
{
    private readonly IPackingNumberingRuleService _service;

    public PackingNumberingRuleController(IPackingNumberingRuleService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.PackingNumberingRule.View)]
    public async Task<ActionResult<ApiResponse<List<PackingNumberingRuleDto>>>> GetAll(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.View)]
    public async Task<ActionResult<ApiResponse<PackingNumberingRuleDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Numbering rule #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpGet("scope/{scope}")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.View)]
    public async Task<ActionResult<ApiResponse<List<PackingNumberingRuleDto>>>> GetByScope(string scope, CancellationToken ct)
    {
        var list = await _service.GetByScopeAsync(scope, ct);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("scope/{scope}/default")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.View)]
    public async Task<ActionResult<ApiResponse<PackingNumberingRuleDto>>> GetDefaultByScope(string scope, CancellationToken ct)
    {
        var item = await _service.GetDefaultByScopeAsync(scope, ct);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Fail($"No default numbering rule found for scope '{scope}'", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpGet("{id:long}/preview")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.View)]
    public async Task<ActionResult<ApiResponse<string>>> PreviewCode(ulong id, CancellationToken ct)
    {
        var result = await _service.PreviewCodeAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.PackingNumberingRule.Add)]
    public async Task<ActionResult<ApiResponse<PackingNumberingRuleDto>>> Create([FromBody] PackingNumberingRuleRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.Edit)]
    public async Task<ActionResult<ApiResponse<PackingNumberingRuleDto>>> Update(ulong id, [FromBody] PackingNumberingRuleRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(true));
    }

    [HttpPost("{id:long}/set-default")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.Edit)]
    public async Task<ActionResult<ApiResponse<bool>>> SetDefault(ulong id, CancellationToken ct)
    {
        var result = await _service.SetDefaultAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(true));
    }

    [HttpPost("generate-codes")]
    [RequirePermission(PermissionCodes.PackingNumberingRule.View)]
    public async Task<ActionResult<ApiResponse<GeneratedPackingCodesDto>>> GenerateCodes([FromBody] GeneratePackingCodeRequest request, CancellationToken ct)
    {
        var result = await _service.GenerateCodesAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }
}
