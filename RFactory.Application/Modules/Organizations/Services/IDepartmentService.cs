using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync(CancellationToken ct = default);
        Task<DepartmentDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DepartmentDto>> CreateAsync(DepartmentRequest request, CancellationToken ct = default);
        Task<Result<DepartmentDto>> UpdateAsync(ulong id, DepartmentRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
