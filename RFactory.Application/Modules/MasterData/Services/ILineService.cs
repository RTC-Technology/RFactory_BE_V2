using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

/// <summary>
/// Application service for the Line master data. Controllers depend on this
/// interface rather than the repository directly, keeping business rules (uniqueness,
/// validation) out of the API layer.
/// </summary>
public interface ILineService
{
    Task<List<LineDto>> GetAllAsync(CancellationToken ct = default);
    Task<LineDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<LineDto>> CreateAsync(CreateLineRequest request, CancellationToken ct = default);
    Task<Result<LineDto>> UpdateAsync(ulong id, UpdateLineRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
