using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.GoodsReceipt.Services;

public interface IGoodsReceiptService
{
    Task<List<GoodsReceiptDto>> GetAllAsync(CancellationToken ct = default);
    Task<GoodsReceiptDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<GoodsReceiptDto>> CreateAsync(CreateGoodsReceiptRequest request, CancellationToken ct = default);
    Task<Result<GoodsReceiptDto>> UpdateAsync(ulong id, UpdateGoodsReceiptRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
