using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.HumanResources.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync(CancellationToken ct = default);
        Task<EmployeeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default);
        Task<Result<EmployeeDto>> UpdateAsync(ulong id, UpdateEmployeeRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}