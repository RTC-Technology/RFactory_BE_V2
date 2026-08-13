using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.GoodsReceipt
{
    /// <summary>
    /// Standalone CRUD for goods receipt lines, gated on <c>goods-receipt-detail.*</c>.
    ///
    /// The receipt screen no longer writes through here — it posts the lines nested in the
    /// receipt so both land in one transaction — but it still reads this endpoint, so the
    /// view code is what the screen's route asks for.
    /// </summary>
    [Route("api/goods-receipt-detail")]
    [ApiController]
    [Authorize]
    public class GoodsReceiptDetailController : ControllerBase
    {
        private readonly IGoodsReceiptDetailServices _goodsReceiptDetailServices;
        private readonly IRepository<GoodsReceiptDetail> _goodsReceiptDetail;

        public GoodsReceiptDetailController(IGoodsReceiptDetailServices goodsReceiptDetailServices, IRepository<GoodsReceiptDetail> goodsReceiptDetail)
        {
            _goodsReceiptDetailServices = goodsReceiptDetailServices;
            _goodsReceiptDetail = goodsReceiptDetail;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.View)]
        public async Task<ActionResult<ApiResponse<List<GoodsReceiptDetailDto>>>> GetAll(long? receiptId,CancellationToken ct)
        {
            var items = await _goodsReceiptDetailServices.GetAllAsync(receiptId, ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.View)]
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
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Add)]
        public async Task<ActionResult<ApiResponse<GoodsReceiptDetailDto>>> Create(CreateGoodsReceiptDetailRequest request, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPost("create-range")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Add)]
        public async Task<ActionResult<ApiResponse<List<GoodsReceiptDetailDto>>>> CreateRange(List<CreateGoodsReceiptDetailRequest> requests, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.CreateRangeAsync(requests, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Edit)]
        public async Task<ActionResult<ApiResponse<GoodsReceiptDetailDto>>> Update(ulong id, UpdatesGoodsReceiptDetailRequest request, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("update-range")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Edit)]
        public async Task<ActionResult<ApiResponse<GoodsReceiptDetailDto>>> UpdateRange( List<UpdatesGoodsReceiptDetailRequest> requests, CancellationToken ct)
        {
            var result = await _goodsReceiptDetailServices.UpdateRangeAsync(requests, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Delete)]
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
