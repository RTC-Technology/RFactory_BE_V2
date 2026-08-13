using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.GoodsReceipt.Services;

public interface IGoodsReceiptDetailService
{
    Task<List<GoodsReceiptDetailDto>> GetAllAsync(long? receiptId, CancellationToken ct = default);
    Task<GoodsReceiptDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<GoodsReceiptDetailDto>> CreateAsync(CreateGoodsReceiptDetailRequest request, CancellationToken ct = default);
    Task<Result<List<GoodsReceiptDetailDto>>> CreateRangeAsync(List<CreateGoodsReceiptDetailRequest> requests, CancellationToken ct = default);
    Task<Result<GoodsReceiptDetailDto>> UpdateAsync(ulong id, UpdatesGoodsReceiptDetailRequest request, CancellationToken ct = default);
    Task<Result<List<GoodsReceiptDetailDto>>> UpdateRangeAsync(List<UpdatesGoodsReceiptDetailRequest> requests, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
