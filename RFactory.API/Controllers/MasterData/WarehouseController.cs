using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData
{
    [Route("api/master-data/warehouse")]
    [ApiController]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Warehouse.View)]
        public async Task<ActionResult<ApiResponse<List<WarehouseDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _warehouseService.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }
    }
}
