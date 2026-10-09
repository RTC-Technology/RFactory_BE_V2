using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ITraceabilityRuleService
    {
        Task<List<TraceabilityRuleDto>> GetAllAsync(CancellationToken ct = default);
        Task<TraceabilityRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<TraceabilityRuleDto>> CreateAsync(TraceabilityRuleRequest request, CancellationToken ct = default);
        Task<Result<TraceabilityRuleDto>> UpdateAsync(ulong id, TraceabilityRuleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ITraceabilityRuleItemService
    {
        Task<List<TraceabilityRuleItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<TraceabilityRuleItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<TraceabilityRuleItemDto>> CreateAsync(TraceabilityRuleItemRequest request, CancellationToken ct = default);
        Task<Result<TraceabilityRuleItemDto>> UpdateAsync(ulong id, TraceabilityRuleItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ITraceabilityTypeService
    {
        Task<List<TraceabilityTypeDto>> GetAllAsync(CancellationToken ct = default);
        Task<TraceabilityTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<TraceabilityTypeDto>> CreateAsync(TraceabilityTypeRequest request, CancellationToken ct = default);
        Task<Result<TraceabilityTypeDto>> UpdateAsync(ulong id, TraceabilityTypeRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
