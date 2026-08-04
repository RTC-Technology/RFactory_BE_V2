using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

/// <summary>
/// Application service for the Organization master data. Controllers depend on this
/// interface rather than the repository directly, keeping business rules (uniqueness,
/// validation) out of the API layer.
/// </summary>
public interface IOrganizationService
{
    Task<List<OrganizationDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrganizationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<OrganizationDto>> CreateAsync(CreateOrganizationRequest request, CancellationToken ct = default);
    Task<Result<OrganizationDto>> UpdateAsync(ulong id, UpdateOrganizationRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
