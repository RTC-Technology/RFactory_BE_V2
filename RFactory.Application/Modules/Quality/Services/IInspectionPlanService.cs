using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.Services
{
    public interface IInspectionPlanService
    {
        Task<List<InspectionPlanDto>> GetAllAsync(CancellationToken ct = default);
        Task<InspectionPlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<InspectionPlanDto>> CreateAsync(InspectionPlanRequest request, CancellationToken ct = default);
        Task<Result<InspectionPlanDto>> UpdateAsync(ulong id, InspectionPlanRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IInspectionItemService
    {
        Task<List<InspectionItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<InspectionItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<InspectionItemDto>> CreateAsync(InspectionItemRequest request, CancellationToken ct = default);
        Task<Result<InspectionItemDto>> UpdateAsync(ulong id, InspectionItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IInspectionExecutionService
    {
        Task<List<InspectionExecutionDto>> GetAllAsync(CancellationToken ct = default);
        Task<InspectionExecutionDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<InspectionExecutionDto>> CreateAsync(InspectionExecutionRequest request, CancellationToken ct = default);
        Task<Result<InspectionExecutionDto>> UpdateAsync(ulong id, InspectionExecutionRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IInspectionResultService
    {
        Task<List<InspectionResultDto>> GetAllAsync(CancellationToken ct = default);
        Task<InspectionResultDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<InspectionResultDto>> CreateAsync(InspectionResultRequest request, CancellationToken ct = default);
        Task<Result<InspectionResultDto>> UpdateAsync(ulong id, InspectionResultRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
