using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for the Line master data.
/// </summary>
[ApiController]
[Route("api/master-data/lines")]
[Authorize]
public class LineController : ControllerBase
{
    private readonly ILineService _lineService;

    public LineController(ILineService lineService)
    {
        _lineService = lineService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<LineDto>>>> GetAll(CancellationToken ct)
    {
        var lines = await _lineService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(lines));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<LineDto>>> GetById(ulong id, CancellationToken ct)
    {
        var line = await _lineService.GetByIdAsync(id, ct);
        if (line is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Line {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(line));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LineDto>>> Create(CreateLineRequest request, CancellationToken ct)
    {
        var result = await _lineService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<LineDto>>> Update(ulong id, UpdateLineRequest request, CancellationToken ct)
    {
        var result = await _lineService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _lineService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Line deleted."));
    }
}
