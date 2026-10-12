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
    [Route("api/master-data/customer/contacts")]
    [ApiController]
    [Authorize]

    public class CustomerContactController : ControllerBase
    {
        private readonly ICustomerContactService _service;

        public CustomerContactController(ICustomerContactService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(PermissionCodes.CustomerContact.View)]
        public async Task<ActionResult<ApiResponse<List<CustomerContactDto>>>> GetAll(CancellationToken ct)
        {
            var contacts = await _service.GetAllAsync(ct);
            return Ok(ApiResponseFactory.Success(contacts));
        }

        [HttpGet("{id:long}")]
        [RequirePermission(PermissionCodes.CustomerContact.View)]
        public async Task<ActionResult<ApiResponse<CustomerContactDto>>> GetById(ulong id, CancellationToken ct)
        {
            var contact = await _service.GetByIdAsync(id, ct);
            if (contact is null)
            {
                return NotFound(ApiResponseFactory.Fail($"CustomerContact {id} was not found.", System.Net.HttpStatusCode.NotFound));
            }

            return Ok(ApiResponseFactory.Success(contact));
        }

        [HttpPost]
        [RequirePermission(PermissionCodes.CustomerContact.Add)]
        public async Task<ActionResult<ApiResponse<CustomerContactDto>>> Create(CustomerContactRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponseFactory.Fail(result.Error!));
            }

            return Ok(ApiResponseFactory.Success(result.Data));
        }

        [HttpPut("{id:long}")]
        [RequirePermission(PermissionCodes.CustomerContact.Edit)]
        public async Task<ActionResult<ApiResponse<CustomerContactDto>>> Update(ulong id, CustomerContactRequest request, CancellationToken ct)
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
