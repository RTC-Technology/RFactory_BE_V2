using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.GoodsIssue.Services;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.GoodsIssue
{
    [Route("api/goods-issue/issues")]
    [ApiController]
    [Authorize]
    public class GoodsIssueController : ControllerBase
    {
        private readonly IGoodsIssueService _goodsIssueServices;

        public GoodsIssueController(IGoodsIssueService goodsIssueServices)
        {
            _goodsIssueServices = goodsIssueServices;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.GoodsIssue.View)]
        public async Task<ActionResult<ApiResponse<List<GoodsIssueDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _goodsIssueServices.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsIssue.View)]
        public async Task<ActionResult<ApiResponse<GoodsIssueDto>>> GetById(ulong id, CancellationToken ct)
        {
            var item = await _goodsIssueServices.GetByIdAsync(id, ct);
            if (item is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Goods Issue {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(item));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.GoodsIssue.Add)]
        public async Task<ActionResult<ApiResponse<GoodsIssueDto>>> Create(CreateGoodsIssueRequest request, CancellationToken ct)
        {
            var result = await _goodsIssueServices.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsIssue.Edit)]
        public async Task<ActionResult<ApiResponse<GoodsIssueDto>>> Update(ulong id, UpdateGoodsIssueRequest request, CancellationToken ct)
        {
            var result = await _goodsIssueServices.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.GoodsIssue.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _goodsIssueServices.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Goods Issue deleted."));
        }
    }
}
