using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData
{
    [Route("api/master-data/customer")]
    [ApiController]
    [Authorize]

    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.Customer.View)]
        public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetAll(CancellationToken ct)
        {
            var customers = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(customers));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.Customer.View)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(ulong id, CancellationToken ct)
        {
            var customer = await _service.GetByIdAsync(id, ct);
            if (customer is null)
            {
                return NotFound(ApiResponseFactory.Fail($"Customer {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(customer));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.Customer.Add)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Create(CustomerRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.Customer.Edit)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(ulong id, CustomerRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpDelete("{id:long}")]
        [RequirePermission(PermissionCodes.Customer.Delete)]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Succeeded)
            {
                return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success<object?>(null, "Customer deleted."));
        }
    }
}
