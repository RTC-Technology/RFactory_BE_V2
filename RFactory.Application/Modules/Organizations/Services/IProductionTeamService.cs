using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.Services
{
    public interface IProductionTeamService
    {
        Task<List<ProductionTeamDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductionTeamDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<ProductionTeamDto>> CreateAsync(ProductionTeamRequest request, CancellationToken ct = default);
        Task<Result<ProductionTeamDto>> UpdateAsync(ulong id, ProductionTeamRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IProductionTeamEmployeeService
    {
        Task<List<ProductionTeamEmployeeDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductionTeamEmployeeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<ProductionTeamEmployeeDto>> CreateAsync(ProductionTeamEmployeeRequest request, CancellationToken ct = default);
        Task<Result<ProductionTeamEmployeeDto>> UpdateAsync(ulong id, ProductionTeamEmployeeRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
