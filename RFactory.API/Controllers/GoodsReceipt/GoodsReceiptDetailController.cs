using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Application.Modules.Product.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.GoodsReceipt
{
    [Route("api/goods-receipt-detail")]
    [ApiController]
    public class GoodsReceiptDetailController : ControllerBase
    {
        private readonly IGoodsReceiptDetailServices _goodsReceiptDetailServices;

        public GoodsReceiptDetailController(IGoodsReceiptDetailServices goodsReceiptDetailServices)
        {
            _goodsReceiptDetailServices = goodsReceiptDetailServices;
        }

        [HttpGet]
        //[RequirePermission(PermissionCodes.BomDetail.View)]
        public async Task<ActionResult<ApiResponse<List<GoodsReceiptDetailDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _goodsReceiptDetailServices.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        //[RequirePermission(PermissionCodes.BomDetail.View)]
        public async Task<ActionResult<ApiResponse<GoodsReceiptDetailDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _goodsReceiptDetailServices.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Good Receipt line {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        //[RequirePermission(PermissionCodes.BomDetail.Add)]
        public async Task<ActionResult<ApiResponse<GoodsReceiptDetailDto>>> Create(CreateGoodsReceiptDetailRequest request, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        //[RequirePermission(PermissionCodes.BomDetail.Edit)]
        public async Task<ActionResult<ApiResponse<GoodsReceiptDetailDto>>> Update(ulong id, UpdatesGoodsReceiptDetailRequest request, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        //[RequirePermission(PermissionCodes.BomDetail.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Goods Receipt Detail line deleted."));
        }
    }
}
