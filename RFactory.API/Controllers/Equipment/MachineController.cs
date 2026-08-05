using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Application.Modules.Equipment.Services;
using RFactory.Shared.Api;

namespace RFactory.API.Controllers.Equipment;

/// <summary>
/// CRUD endpoints for machines.
/// </summary>
[ApiController]
[Route("api/equipment/machines")]
[Authorize]
public class MachineController : ControllerBase
{
    private readonly IMachineService _machineService;

    public MachineController(IMachineService machineService)
    {
        _machineService = machineService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MachineDto>>>> GetAll(CancellationToken ct)
    {
        var machines = await _machineService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(machines));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<MachineDto>>> GetById(ulong id, CancellationToken ct)
    {
        var machine = await _machineService.GetByIdAsync(id, ct);
        if (machine is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Machine {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(machine));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MachineDto>>> Create(CreateMachineRequest request, CancellationToken ct)
    {
        var result = await _machineService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<MachineDto>>> Update(ulong id, UpdateMachineRequest request, CancellationToken ct)
    {
        var result = await _machineService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _machineService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Machine deleted."));
    }
}
