
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Organizations.Services
{
    public interface IWorkshopService
    {
        Task<List<WorkshopDto>> GetAllAsync(CancellationToken ct = default);
        Task<WorkshopDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<WorkshopDto>> CreateAsync(WorkshopRequest request, CancellationToken ct = default);
        Task<Result<WorkshopDto>> UpdateAsync(ulong id, WorkshopRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
