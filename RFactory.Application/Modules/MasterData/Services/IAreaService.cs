using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

/// <summary>
/// Application service for the Area master data. Controllers depend on this
/// interface rather than the repository directly, keeping business rules (uniqueness,
/// validation) out of the API layer.
/// </summary>
public interface IAreaService
{
    Task<List<AreaDto>> GetAllAsync(CancellationToken ct = default);
    Task<AreaDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<AreaDto>> CreateAsync(CreateAreaRequest request, CancellationToken ct = default);
    Task<Result<AreaDto>> UpdateAsync(ulong id, UpdateAreaRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
