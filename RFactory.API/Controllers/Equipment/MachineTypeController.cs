using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Application.Modules.Equipment.Services;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers.Equipment;

/// <summary>
/// CRUD endpoints for machine types.
/// </summary>
[ApiController]
[Route("api/equipment/machine-types")]
[Authorize]
public class MachineTypeController : ControllerBase
{
    private readonly IMachineTypeService _machineTypeService;

    public MachineTypeController(IMachineTypeService machineTypeService)
    {
        _machineTypeService = machineTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MachineTypeDto>>>> GetAll(CancellationToken ct)
    {
        var machineTypes = await _machineTypeService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(machineTypes));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<MachineTypeDto>>> GetById(ulong id, CancellationToken ct)
    {
        var machineType = await _machineTypeService.GetByIdAsync(id, ct);
        if (machineType is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Machine type {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(machineType));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MachineTypeDto>>> Create(CreateMachineTypeRequest request, CancellationToken ct)
    {
        var result = await _machineTypeService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<MachineTypeDto>>> Update(ulong id, UpdateMachineTypeRequest request, CancellationToken ct)
    {
        var result = await _machineTypeService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _machineTypeService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Machine type deleted."));
    }
}
