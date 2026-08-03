using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for the Factory master data.
/// </summary>
[ApiController]
[Route("api/master-data/factories")]
[Authorize]
public class FactoryController : ControllerBase
{
    private readonly IFactoryService _factoryService;

    public FactoryController(IFactoryService factoryService)
    {
        _factoryService = factoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<FactoryDto>>>> GetAll(CancellationToken ct)
    {
        var factories = await _factoryService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(factories));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<FactoryDto>>> GetById(ulong id, CancellationToken ct)
    {
        var factory = await _factoryService.GetByIdAsync(id, ct);
        if (factory is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Factory {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(factory));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FactoryDto>>> Create(CreateFactoryRequest request, CancellationToken ct)
    {
        var result = await _factoryService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<FactoryDto>>> Update(ulong id, UpdateFactoryRequest request, CancellationToken ct)
    {
        var result = await _factoryService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _factoryService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Factory deleted."));
    }
}
