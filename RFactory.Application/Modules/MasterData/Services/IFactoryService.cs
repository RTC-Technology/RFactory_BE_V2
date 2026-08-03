using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

/// <summary>
/// Application service for the Factory master data. Controllers depend on this
/// interface rather than the repository directly, keeping business rules (uniqueness,
/// validation) out of the API layer.
/// </summary>
public interface IFactoryService
{
    Task<List<FactoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<FactoryDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<FactoryDto>> CreateAsync(CreateFactoryRequest request, CancellationToken ct = default);
    Task<Result<FactoryDto>> UpdateAsync(ulong id, UpdateFactoryRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
