using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Application.Modules.Production.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.Production;

[ApiController]
[Route("api/production-orders")]
[Authorize]
public class ProductionOrderController : ControllerBase
{
    private readonly IProductionOrderService _service;

    public ProductionOrderController(IProductionOrderService service)
    {
        _service = service;
    }

    private string CurrentUser => User.Identity?.Name ?? "SYSTEM";

    [HttpGet]
    [RequirePermission(PermissionCodes.ProductionOrder.View)]
    public async Task<ActionResult<ApiResponse<List<ProductionOrderDto>>>> GetAll([FromQuery] ProductionOrderFilterDto filter)
    {
        var list = await _service.GetAllAsync(filter);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.ProductionOrder.View)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> GetById(ulong id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Production order #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    /// <summary>
    /// Dự tính chi tiết NVL theo BOM và số lượng sản xuất ("SL theo định mức = Định mức x SL sản xuất")
    /// </summary>
    [HttpGet("preview-bom")]
    [RequirePermission(PermissionCodes.ProductionOrder.View)]
    public async Task<ActionResult<ApiResponse<List<ProductionOrderMaterialDto>>>> PreviewBomMaterials([FromQuery] ulong bomId, [FromQuery] decimal quantity)
    {
        var items = await _service.PreviewBomMaterialsAsync(bomId, quantity);
        return Ok(ApiResponseFactory.Success(items));
    }

    /// <summary>
    /// Dự tính danh sách công đoạn theo quy trình sản xuất (Routing)
    /// </summary>
    [HttpGet("preview-routing")]
    [RequirePermission(PermissionCodes.ProductionOrder.View)]
    public async Task<ActionResult<ApiResponse<List<ProductionOrderOperationDto>>>> PreviewRoutingOperations([FromQuery] ulong routingId)
    {
        var items = await _service.PreviewRoutingOperationsAsync(routingId);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.ProductionOrder.Add)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Create([FromBody] ProductionOrderCreateRequest request)
    {
        var item = await _service.CreateAsync(request, CurrentUser);
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Update(ulong id, [FromBody] ProductionOrderUpdateRequest request)
    {
        var item = await _service.UpdateAsync(id, request, CurrentUser);
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.ProductionOrder.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(ulong id)
    {
        var result = await _service.DeleteAsync(id, CurrentUser);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpPost("{id:long}/status")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> ChangeStatus(ulong id, [FromBody] ProductionOrderStatusRequest request)
    {
        var item = await _service.ChangeStatusAsync(id, request.Status, CurrentUser, request.Remark);
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/release")]
    [RequirePermission(PermissionCodes.ProductionOrder.Approve)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Release(ulong id)
    {
        var item = await _service.ChangeStatusAsync(id, 2, CurrentUser, "Released for production");
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/start")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Start(ulong id)
    {
        var item = await _service.ChangeStatusAsync(id, 3, CurrentUser, "Production started");
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/pause")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Pause(ulong id, [FromBody] ProductionOrderStatusRequest? request)
    {
        var item = await _service.ChangeStatusAsync(id, 7, CurrentUser, request?.Remark ?? "Production paused");
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/complete")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Complete(ulong id)
    {
        var item = await _service.ChangeStatusAsync(id, 4, CurrentUser, "Production completed");
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/cancel")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Cancel(ulong id, [FromBody] ProductionOrderStatusRequest? request)
    {
        var item = await _service.ChangeStatusAsync(id, 5, CurrentUser, request?.Remark ?? "Production cancelled");
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/close")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Close(ulong id)
    {
        var item = await _service.ChangeStatusAsync(id, 6, CurrentUser, "Production order closed");
        return Ok(ApiResponseFactory.Success(item));
    }

    /// <summary>
    /// Lấy lại chi tiết NVL theo BOM ban đầu ("Lấy lại theo BOM")
    /// </summary>
    [HttpPost("{id:long}/reset-bom")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> ResetBom(ulong id)
    {
        var item = await _service.ResetMaterialsToBomAsync(id, CurrentUser);
        return Ok(ApiResponseFactory.Success(item));
    }

    /// <summary>
    /// Nhân bản lệnh sản xuất (Duplicate)
    /// </summary>
    [HttpPost("{id:long}/duplicate")]
    [RequirePermission(PermissionCodes.ProductionOrder.Add)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> Duplicate(ulong id)
    {
        var item = await _service.DuplicateAsync(id, CurrentUser);
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost("{id:long}/progress")]
    [RequirePermission(PermissionCodes.ProductionOrder.Edit)]
    public async Task<ActionResult<ApiResponse<ProductionOrderDto>>> RecordProgress(ulong id, [FromBody] ProductionOrderProgressRequest request)
    {
        var item = await _service.RecordProgressAsync(id, request, CurrentUser);
        return Ok(ApiResponseFactory.Success(item));
    }
}
