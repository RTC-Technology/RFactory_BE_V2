using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.HumanResources.Services;

public interface ISkillService
{
    Task<List<SkillDto>> GetAllAsync(CancellationToken ct = default);
    Task<SkillDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<SkillDto>> CreateAsync(CreateSkillRequest request, CancellationToken ct = default);
    Task<Result<SkillDto>> UpdateAsync(ulong id, UpdateSkillRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
