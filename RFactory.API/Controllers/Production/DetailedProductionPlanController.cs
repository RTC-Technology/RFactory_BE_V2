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
[Route("api/detailed-production-plans")]
[Authorize]
public class DetailedProductionPlanController : ControllerBase
{
    private readonly IDetailedProductionPlanService _service;

    public DetailedProductionPlanController(IDetailedProductionPlanService service)
    {
        _service = service;
    }

    private string CurrentUser => User.Identity?.Name ?? "SYSTEM";

    [HttpGet]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.View)]
    public async Task<ActionResult<ApiResponse<List<DetailedProductionPlanDto>>>> GetAll([FromQuery] DetailedProductionPlanFilterDto filter)
    {
        var list = await _service.GetAllAsync(filter);
        return Ok(ApiResponseFactory.Success(list));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.View)]
    public async Task<ActionResult<ApiResponse<DetailedProductionPlanDto>>> GetById(ulong id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Detailed production plan #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.Add)]
    public async Task<ActionResult<ApiResponse<DetailedProductionPlanDto>>> Create([FromBody] DetailedProductionPlanCreateRequest request)
    {
        var created = await _service.CreateAsync(request, CurrentUser);
        return StatusCode((int)HttpStatusCode.Created, ApiResponseFactory.Success(created));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.Edit)]
    public async Task<ActionResult<ApiResponse<DetailedProductionPlanDto>>> Update(ulong id, [FromBody] DetailedProductionPlanUpdateRequest request)
    {
        var updated = await _service.UpdateAsync(id, request, CurrentUser);
        return Ok(ApiResponseFactory.Success(updated));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(ulong id)
    {
        var result = await _service.DeleteAsync(id, CurrentUser);
        if (!result)
        {
            return NotFound(ApiResponseFactory.Fail($"Detailed production plan #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(true));
    }

    [HttpPut("{id:long}/status")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.Approve)]
    public async Task<ActionResult<ApiResponse<DetailedProductionPlanDto>>> ChangeStatus(ulong id, [FromBody] ChangePlanStatusRequest request)
    {
        var result = await _service.ChangeStatusAsync(id, request.Status, CurrentUser, request.Remark);
        return Ok(ApiResponseFactory.Success(result));
    }

    [HttpGet("master-plans")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.View)]
    public async Task<ActionResult<ApiResponse<List<MasterProductionPlanDto>>>> GetMasterPlans()
    {
        var plans = await _service.GetMasterPlansAsync();
        return Ok(ApiResponseFactory.Success(plans));
    }

    [HttpGet("master-plans/{id:long}")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.View)]
    public async Task<ActionResult<ApiResponse<MasterProductionPlanDto>>> GetMasterPlanById(ulong id)
    {
        var plan = await _service.GetMasterPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(ApiResponseFactory.Fail($"Master production plan #{id} not found", HttpStatusCode.NotFound));
        }
        return Ok(ApiResponseFactory.Success(plan));
    }

    /// <summary>
    /// Thuật toán tự động phân bổ (Tiến / Lùi) theo ngày hoặc ca
    /// </summary>
    [HttpPost("calculate-allocation")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.View)]
    public ActionResult<ApiResponse<PlanAllocationCalculationResultDto>> CalculateAllocation([FromBody] PlanAllocationCalculationRequest request)
    {
        var result = _service.CalculateAllocation(request);
        return Ok(ApiResponseFactory.Success(result));
    }

    /// <summary>
    /// Kiểm tra số lượng sản xuất đã phân bổ so với nhu cầu ("Kiểm tra SL sản xuất")
    /// </summary>
    [HttpPost("validate-quantities")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.View)]
    public ActionResult<ApiResponse<ValidateQuantitiesResultDto>> ValidateQuantities([FromBody] ValidateQuantitiesRequest request)
    {
        var result = _service.ValidateQuantities(request);
        return Ok(ApiResponseFactory.Success(result));
    }

    /// <summary>
    /// Lập Lệnh sản xuất từ Kế hoạch chi tiết (AMIS Bước 4)
    /// </summary>
    [HttpPost("generate-orders")]
    [RequirePermission(PermissionCodes.DetailedProductionPlan.Approve)]
    public async Task<ActionResult<ApiResponse<GenerateOrdersResultDto>>> GenerateProductionOrders([FromBody] GenerateOrdersFromPlanRequest request)
    {
        var result = await _service.GenerateProductionOrdersAsync(request, CurrentUser);
        return Ok(ApiResponseFactory.Success(result));
    }
}

public class ChangePlanStatusRequest
{
    public int Status { get; set; }
    public string? Remark { get; set; }
}
