using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData
{
    [Route("api/master-data/location")]
    [ApiController]
    [Authorize]
    public class WarehouseLocationController : ControllerBase
    {
        private readonly IWarehouseLocationService _locationService;

        public WarehouseLocationController(IWarehouseLocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Location.View)]
        public async Task<ActionResult<ApiResponse<List<WarehouseLocationDto>>>> GetAll(CancellationToken ct)
        {
            var items = await _locationService.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(items));
        }
    }
}
