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
    [Route("api/goods-receipt")]
    [ApiController]
    public class GoodsReceiptController : ControllerBase
    {
        private readonly IGoodsReceiptServices _goodsReceiptServices;

        public GoodsReceiptController(IGoodsReceiptServices goodsReceiptServices)
        {
            _goodsReceiptServices = goodsReceiptServices;
        }

        [HttpGet]
        //[RequirePermission(PermissionCodes.Bom.View)]
        public async Task<ActionResult<ApiResponse<List<GoodsReceiptDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _goodsReceiptServices.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        //[RequirePermission(PermissionCodes.Bom.View)]
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
        //[RequirePermission(PermissionCodes.Bom.Add)]
        public async Task<ActionResult<ApiResponse<BomDto>>> Create(CreateGoodsReceiptRequest request, CancellationToken ct)
        {
            var result = await _goodsReceiptServices.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        //[RequirePermission(PermissionCodes.Bom.Edit)]
        public async Task<ActionResult<ApiResponse<BomDto>>> Update(ulong id, UpdateGoodsReceiptRequest request, CancellationToken ct)
        {
            var result = await _goodsReceiptServices.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Bom.Delete)]
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
}
