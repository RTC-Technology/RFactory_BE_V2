using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.GoodsReceipt;

/// <summary>
/// CRUD endpoints for goods receipts.
///
/// The write actions gate on <c>goods-receipt.*</c> alone even though POST and PUT also
/// write the receipt's lines: the lines arrive nested in the same payload and are saved
/// in one transaction, so they are part of the receipt aggregate rather than a separate
/// thing to authorize. <c>goods-receipt-detail.*</c> gates the standalone line endpoints.
/// </summary>
[Route("api/goods-receipt/receipts")]
[ApiController]
[Authorize]
public class GoodsReceiptController : ControllerBase
{
    private readonly IGoodsReceiptService _goodsReceiptServices;

    public GoodsReceiptController(IGoodsReceiptService goodsReceiptServices)
    {
        _goodsReceiptServices = goodsReceiptServices;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.GoodsReceipt.View)]
    public async Task<ActionResult<ApiResponse<List<GoodsReceiptDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _goodsReceiptServices.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(items));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.GoodsReceipt.View)]
    public async Task<ActionResult<ApiResponse<GoodsReceiptDto>>> GetById(ulong id, CancellationToken ct)
    {
        var item = await _goodsReceiptServices.GetByIdAsync(id, ct);
        if (item is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Goods Receipt {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(item));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.GoodsReceipt.Add)]
    public async Task<ActionResult<ApiResponse<GoodsReceiptDto>>> Create(CreateGoodsReceiptRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _goodsReceiptServices.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.GoodsReceipt.Edit)]
    public async Task<ActionResult<ApiResponse<GoodsReceiptDto>>> Update(ulong id, UpdateGoodsReceiptRequest request, CancellationToken ct)
    {
        var result = await _goodsReceiptServices.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.GoodsReceipt.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _goodsReceiptServices.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Goods Receipt deleted."));
    }
}
