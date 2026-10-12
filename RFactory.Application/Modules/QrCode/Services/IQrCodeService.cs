using RFactory.Application.Modules.QrCode.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.QrCode.Services;

public interface IQrCodeService
{
    Task<Result<QrCodeDto>> GenerateAsync(QrCodeGenerateRequest request, CancellationToken ct = default);
    Task<Result<QrScanResultDto>> ScanAndLookupAsync(string payload, CancellationToken ct = default);
    Task<Result<List<QrItemLookupDto>>> GetAvailableItemsAsync(string entityType, CancellationToken ct = default);
    Task<Result<List<QrCodeDto>>> BatchGenerateAsync(QrBatchPrintRequest request, CancellationToken ct = default);
}
