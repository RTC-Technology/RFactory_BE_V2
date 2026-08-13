using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Warehouse.Services;
using RFactory.Application.Modules.Warehouses.DTOs;

namespace RFactory.API.Controllers.Warehouse;

[ApiController]
[Route("api/warehouse-locations")]
public class WarehouseLocationController : ControllerBase
{
    private readonly IWarehouseLocationService _service;

    public WarehouseLocationController(IWarehouseLocationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CancellationToken ct = default)
    {
        var locations = await _service.GetAllAsync(ct);
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(ulong id, [FromQuery] CancellationToken ct = default)
    {
        var location = await _service.GetByIdAsync(id, ct);
        return location is not null ? Ok(location) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseLocationRequest request, [FromQuery] CancellationToken ct = default)
    {
        var result = await _service.CreateAsync(request, ct);
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateWarehouseLocationRequest request, [FromQuery] CancellationToken ct = default)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(ulong id, [FromQuery] CancellationToken ct = default)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.Succeeded ? NoContent() : NotFound();
    }
}