using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers.Administration;

/// <summary>
/// CRUD endpoints for functions (permissions).
/// </summary>
[ApiController]
[Route("api/administration/functions")]
[Authorize]
public class FunctionController : ControllerBase
{
    private readonly IFunctionService _functionService;

    public FunctionController(IFunctionService functionService)
    {
        _functionService = functionService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<FunctionDto>>>> GetAll(CancellationToken ct)
    {
        var functions = await _functionService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(functions));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<FunctionDto>>> GetById(ulong id, CancellationToken ct)
    {
        var function = await _functionService.GetByIdAsync(id, ct);
        if (function is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Function {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(function));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FunctionDto>>> Create(CreateFunctionRequest request, CancellationToken ct)
    {
        var result = await _functionService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<FunctionDto>>> Update(ulong id, UpdateFunctionRequest request, CancellationToken ct)
    {
        var result = await _functionService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _functionService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Function deleted."));
    }
}
