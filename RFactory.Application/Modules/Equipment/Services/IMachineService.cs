using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Equipment.Services;

/// <summary>
/// Application service for machines. Controllers depend on this interface rather than
/// the repository directly, keeping business rules (uniqueness, validation) out of the API layer.
/// </summary>
public interface IMachineService
{
    Task<List<MachineDto>> GetAllAsync(CancellationToken ct = default);
    Task<MachineDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<MachineDto>> CreateAsync(CreateMachineRequest request, CancellationToken ct = default);
    Task<Result<MachineDto>> UpdateAsync(ulong id, UpdateMachineRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
