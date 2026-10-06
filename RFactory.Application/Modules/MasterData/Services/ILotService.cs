using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ILotService
    {
        Task<List<LotDto>> GetAllAsync(CancellationToken ct = default);
        Task<LotDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<LotDto>> CreateAsync(LotRequest request, CancellationToken ct = default);
        Task<Result<LotDto>> UpdateAsync(ulong id, LotRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ILotRuleService
    {
        Task<List<LotRuleDto>> GetAllAsync(CancellationToken ct = default);
        Task<LotRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<LotRuleDto>> CreateAsync(LotRuleRequest request, CancellationToken ct = default);
        Task<Result<LotRuleDto>> UpdateAsync(ulong id, LotRuleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ILotRuleSequenceService
    {
        Task<List<LotRuleSequenceDto>> GetAllAsync(CancellationToken ct = default);
        Task<LotRuleSequenceDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<LotRuleSequenceDto>> CreateAsync(LotRuleSequenceRequest request, CancellationToken ct = default);
        Task<Result<LotRuleSequenceDto>> UpdateAsync(ulong id, LotRuleSequenceRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IProductLotRuleService
    {
        Task<List<ProductLotRuleDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductLotRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<ProductLotRuleDto>> CreateAsync(ProductLotRuleRequest request, CancellationToken ct = default);
        Task<Result<ProductLotRuleDto>> UpdateAsync(ulong id, ProductLotRuleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
