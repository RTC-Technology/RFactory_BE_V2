using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData
{
    [Route("api/master-data/traceability/types")]
    [ApiController]
    public class TraceabilityTypeController : ControllerBase
    {
        private readonly ITraceabilityTypeService _service;

        public TraceabilityTypeController(ITraceabilityTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.TraceabilityType.View)]
        public async Task<ActionResult<ApiResponse<List<TraceabilityTypeDto>>>> GetAll(CancellationToken ct)
        {
            var traceabilityTypes = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(traceabilityTypes));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityType.View)]
        public async Task<ActionResult<ApiResponse<TraceabilityTypeDto>>> GetById(ulong id, CancellationToken ct)
        {
            var traceabilityType = await _service.GetByIdAsync(id, ct);
            if (traceabilityType is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Traceability Type {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(traceabilityType));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.TraceabilityType.Add)]
        public async Task<ActionResult<ApiResponse<TraceabilityTypeDto>>> Create(TraceabilityTypeRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityType.Edit)]
        public async Task<ActionResult<ApiResponse<TraceabilityTypeDto>>> Update(ulong id, TraceabilityTypeRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.TraceabilityType.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Traceability Type deleted."));
        }
    }
}
