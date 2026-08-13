using Microsoft.AspNetCore.Mvc;
using RFactory.Application.Modules.Warehouse.Services;
using RFactory.Application.Modules.Warehouses.DTOs;

namespace RFactory.API.Controllers.Warehouse;

[ApiController]
[Route("api/warehouses")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehouseController(IWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CancellationToken ct = default)
    {
        var warehouses = await _service.GetAllAsync(ct);
        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(ulong id, [FromQuery] CancellationToken ct = default)
    {
        var warehouse = await _service.GetByIdAsync(id, ct);
        return warehouse is not null ? Ok(warehouse) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request, [FromQuery] CancellationToken ct = default)
    {
        var result = await _service.CreateAsync(request, ct);
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateWarehouseRequest request, [FromQuery] CancellationToken ct = default)
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