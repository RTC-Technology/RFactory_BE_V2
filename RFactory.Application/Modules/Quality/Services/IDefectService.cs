using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.Services
{
    public interface IDefectService
    {
        Task<List<DefectDto>> GetAllAsync(CancellationToken ct = default);
        Task<DefectDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DefectDto>> CreateAsync(DefectRequest request, CancellationToken ct = default);
        Task<Result<DefectDto>> UpdateAsync(ulong id, DefectRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IDefectGroupService
    {
        Task<List<DefectGroupDto>> GetAllAsync(CancellationToken ct = default);
        Task<DefectGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<DefectGroupDto>> CreateAsync(DefectGroupRequest request, CancellationToken ct = default);
        Task<Result<DefectGroupDto>> UpdateAsync(ulong id, DefectGroupRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
