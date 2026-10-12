using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public interface ILabelTemplateService
{
    Task<List<LabelTemplateDto>> GetAllAsync(CancellationToken ct = default);
    Task<LabelTemplateDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<List<LabelTemplateDto>> GetByEntityTypeAsync(string entityType, CancellationToken ct = default);
    Task<LabelTemplateDto?> GetDefaultByEntityTypeAsync(string entityType, CancellationToken ct = default);
    Task<Result<LabelTemplateDto>> CreateAsync(LabelTemplateRequest request, CancellationToken ct = default);
    Task<Result<LabelTemplateDto>> UpdateAsync(ulong id, LabelTemplateRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(ulong id, CancellationToken ct = default);
    Task<Result<bool>> SetDefaultAsync(ulong id, CancellationToken ct = default);
}
