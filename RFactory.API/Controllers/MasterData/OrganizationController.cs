using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.MasterData;

/// <summary>
/// CRUD endpoints for the Organization master data.
/// </summary>
[ApiController]
[Route("api/master-data/organizations")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet]
    [RequirePermission(PermissionCodes.Organization.View)]
    public async Task<ActionResult<ApiResponse<List<OrganizationDto>>>> GetAll(CancellationToken ct)
    {
        var organizations = await _organizationService.GetAllAsync(ct);
        return Ok(ApiResponseFactory.Success(organizations));
    }

    [HttpGet("{id:long}")]
    [RequirePermission(PermissionCodes.Organization.View)]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> GetById(ulong id, CancellationToken ct)
    {
        var organization = await _organizationService.GetByIdAsync(id, ct);
        if (organization is null)
        {
            return NotFound(ApiResponseFactory.Fail($"Organization {id} was not found.", System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(organization));
    }

    [HttpPost]
    [RequirePermission(PermissionCodes.Organization.Add)]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Create(CreateOrganizationRequest request, CancellationToken ct)
    {
        var result = await _organizationService.CreateAsync(request, ct);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponseFactory.Fail(result.Error!));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpPut("{id:long}")]
    [RequirePermission(PermissionCodes.Organization.Edit)]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Update(ulong id, UpdateOrganizationRequest request, CancellationToken ct)
    {
        var result = await _organizationService.UpdateAsync(id, request, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success(result.Data));
    }

    [HttpDelete("{id:long}")]
    [RequirePermission(PermissionCodes.Organization.Delete)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(ulong id, CancellationToken ct)
    {
        var result = await _organizationService.DeleteAsync(id, ct);
        if (!result.Succeeded)
        {
            return NotFound(ApiResponseFactory.Fail(result.Error!, System.Net.HttpStatusCode.NotFound));
        }

        return Ok(ApiResponseFactory.Success<object?>(null, "Organization deleted."));
    }
}
