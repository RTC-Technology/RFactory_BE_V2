using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Equipment.Services;

/// <summary>
/// Application service for machine types. Controllers depend on this interface rather
/// than the repository directly, keeping business rules (uniqueness, validation) out
/// of the API layer.
/// </summary>
public interface IMachineTypeService
{
    Task<List<MachineTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<MachineTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<MachineTypeDto>> CreateAsync(CreateMachineTypeRequest request, CancellationToken ct = default);
    Task<Result<MachineTypeDto>> UpdateAsync(ulong id, UpdateMachineTypeRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
