using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// Application service for function groups (permission groups).
/// </summary>
public interface IFunctionGroupService
{
    Task<List<FunctionGroupDto>> GetAllAsync(CancellationToken ct = default);
    Task<FunctionGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<FunctionGroupDto>> CreateAsync(CreateFunctionGroupRequest request, CancellationToken ct = default);
    Task<Result<FunctionGroupDto>> UpdateAsync(ulong id, UpdateFunctionGroupRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
