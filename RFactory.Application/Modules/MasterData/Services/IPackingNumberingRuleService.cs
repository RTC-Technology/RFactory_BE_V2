using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public interface IPackingNumberingRuleService
{
    Task<List<PackingNumberingRuleDto>> GetAllAsync(CancellationToken ct = default);
    Task<PackingNumberingRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<List<PackingNumberingRuleDto>> GetByScopeAsync(string scope, CancellationToken ct = default);
    Task<PackingNumberingRuleDto?> GetDefaultByScopeAsync(string scope, CancellationToken ct = default);
    Task<Result<PackingNumberingRuleDto>> CreateAsync(PackingNumberingRuleRequest request, CancellationToken ct = default);
    Task<Result<PackingNumberingRuleDto>> UpdateAsync(ulong id, PackingNumberingRuleRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(ulong id, CancellationToken ct = default);
    Task<Result<bool>> SetDefaultAsync(ulong id, CancellationToken ct = default);
    Task<Result<GeneratedPackingCodesDto>> GenerateCodesAsync(GeneratePackingCodeRequest request, CancellationToken ct = default);
    Task<Result<string>> PreviewCodeAsync(ulong ruleId, CancellationToken ct = default);
}
