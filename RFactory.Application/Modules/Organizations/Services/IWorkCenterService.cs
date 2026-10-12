using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.Services
{
    public interface IWorkCenterService
    {
        Task<List<WorkCenterDto>> GetAllAsync(CancellationToken ct = default);
        Task<WorkCenterDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<WorkCenterDto>> CreateAsync(WorkCenterRequest request, CancellationToken ct = default);
        Task<Result<WorkCenterDto>> UpdateAsync(ulong id, WorkCenterRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
