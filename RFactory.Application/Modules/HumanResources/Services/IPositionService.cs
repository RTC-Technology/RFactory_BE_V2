using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.HumanResources.Services;

public interface IPositionService
{
    Task<List<PositionDto>> GetAllAsync(CancellationToken ct = default);
    Task<PositionDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<PositionDto>> CreateAsync(CreatePositionRequest request, CancellationToken ct = default);
    Task<Result<PositionDto>> UpdateAsync(ulong id, UpdatePositionRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
