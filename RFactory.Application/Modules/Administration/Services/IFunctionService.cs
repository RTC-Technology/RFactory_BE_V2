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
}
