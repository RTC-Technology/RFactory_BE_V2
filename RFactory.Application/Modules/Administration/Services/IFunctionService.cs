using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// Application service for functions (permissions).
/// </summary>
public interface IFunctionService
{
    Task<List<FunctionDto>> GetAllAsync(CancellationToken ct = default);
    Task<FunctionDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<FunctionDto>> CreateAsync(CreateFunctionRequest request, CancellationToken ct = default);
    Task<Result<FunctionDto>> UpdateAsync(ulong id, UpdateFunctionRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);

    /// <summary>
    /// Creates any group or permission from the application's catalogue that the database
    /// does not have yet. Additive only — nothing is renamed or removed, so existing ids
    /// and the rights assigned against them stay intact.
    /// </summary>
    Task<Result<PermissionSyncResult>> SyncCatalogAsync(CancellationToken ct = default);
}
