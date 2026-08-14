using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.GoodsIssue.Services;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.GoodsIssue
{
    [Route("api/goods-issue/details")]
    [ApiController]
    public class GoodsIssueDetailController : ControllerBase
    {
        private readonly IGoodsIssueDetailService _service;
        private readonly IRepository<GoodsIssueDetail> _goodsReceiptDetail;

        public GoodsIssueDetailController(IGoodsIssueDetailService service, IRepository<GoodsIssueDetail> goodsReceiptDetail)
        {
            _service = service;
            _goodsReceiptDetail = goodsReceiptDetail;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.View)]
        public async Task<ActionResult<ApiResponse<List<GoodsIssueDetailDto>>>> GetAll(long? issueId, CancellationToken ct)
        {
            var items = await _service.GetAllAsync(issueId, ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.View)]
        public async Task<ActionResult<ApiResponse<GoodsIssueDetailDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Good Issue line {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Add)]
        public async Task<ActionResult<ApiResponse<GoodsIssueDetailDto>>> Create(GoodsIssueDetailRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsReceiptDetail.Edit)]
        public async Task<ActionResult<ApiResponse<GoodsIssueDetailDto>>> Update(ulong id, GoodsIssueDetailRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
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
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Goods Issue Detail line deleted."));
        }
    }
}
