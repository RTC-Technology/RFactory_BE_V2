using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Quality.Services
{
    public interface ISamplingPlanService
    {
        Task<List<SamplingPlanDto>> GetAllAsync(CancellationToken ct = default);
        Task<SamplingPlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SamplingPlanDto>> CreateAsync(SamplingPlanRequest request, CancellationToken ct = default);
        Task<Result<SamplingPlanDto>> UpdateAsync(ulong id, SamplingPlanRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ISamplingPlanRuleService
    {
        Task<List<SamplingPlanRuleDto>> GetAllAsync(CancellationToken ct = default);
        Task<SamplingPlanRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SamplingPlanRuleDto>> CreateAsync(SamplingPlanRuleRequest request, CancellationToken ct = default);
        Task<Result<SamplingPlanRuleDto>> UpdateAsync(ulong id, SamplingPlanRuleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
