using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

/// <summary>Working shifts.</summary>
public interface IShiftService
{
    Task<List<ShiftDto>> GetAllAsync(CancellationToken ct = default);
    Task<ShiftDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<ShiftDto>> CreateAsync(CreateShiftRequest request, CancellationToken ct = default);
    Task<Result<ShiftDto>> UpdateAsync(ulong id, UpdateShiftRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

/// <summary>Breaks belonging to a shift.</summary>
public interface IShiftBreakService
{
    Task<List<ShiftBreakDto>> GetAllAsync(CancellationToken ct = default);
    Task<ShiftBreakDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<ShiftBreakDto>> CreateAsync(CreateShiftBreakRequest request, CancellationToken ct = default);
    Task<Result<ShiftBreakDto>> UpdateAsync(ulong id, UpdateShiftBreakRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
