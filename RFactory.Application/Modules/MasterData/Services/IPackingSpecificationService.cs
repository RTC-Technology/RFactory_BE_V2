using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public interface IPackingSpecificationService
{
    Task<List<PackingSpecificationDto>> GetAllAsync(CancellationToken ct = default);
    Task<PackingSpecificationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<List<PackingSpecificationDto>> GetByProductIdAsync(ulong productId, CancellationToken ct = default);
    Task<PackingSpecificationDto?> GetDefaultByProductIdAsync(ulong productId, CancellationToken ct = default);
    Task<Result<PackingSpecificationDto>> CreateAsync(PackingSpecificationRequest request, CancellationToken ct = default);
    Task<Result<PackingSpecificationDto>> UpdateAsync(ulong id, PackingSpecificationRequest request, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(ulong id, CancellationToken ct = default);
    Task<Result<bool>> SetDefaultAsync(ulong id, CancellationToken ct = default);
}
