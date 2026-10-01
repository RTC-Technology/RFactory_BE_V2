using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.HumanResources.Services;

public interface IEmployeeSkillService
{
    Task<List<EmployeeSkillDto>> GetAllAsync(CancellationToken ct = default);
    Task<List<EmployeeSkillDto>> GetByEmployeeAsync(ulong employeeId, CancellationToken ct = default);
    Task<EmployeeSkillDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<EmployeeSkillDto>> CreateAsync(CreateEmployeeSkillRequest request, CancellationToken ct = default);
    Task<Result<EmployeeSkillDto>> UpdateAsync(ulong id, UpdateEmployeeSkillRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
