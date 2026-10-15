using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFactory.API.Authorization;
using RFactory.Application.Modules.QrCode.DTOs;
using RFactory.Application.Modules.QrCode.Services;
using RFactory.Shared.Api;
using RFactory.Shared.Constants;

namespace RFactory.API.Controllers.QrCode;

/// <summary>
/// Endpoints for generating, printing, and scanning QR codes across MES entities.
/// </summary>
[ApiController]
[Route("api/qrcode")]
[Authorize]
public class QrCodeController : ControllerBase
{
    private readonly IQrCodeService _service;

    public QrCodeController(IQrCodeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Generates QR payload and label metadata for a target entity or custom text.
    /// </summary>
    [HttpPost("generate")]
    [RequirePermission(PermissionCodes.QrCode.Generate)]
    public async Task<ActionResult<ApiResponse<QrCodeDto>>> Generate([FromBody] QrCodeGenerateRequest request, CancellationToken ct)
    {
        var result = await _service.GenerateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<QrCodeDto>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    /// <summary>
    /// Scans a QR payload string, parses entity type/ID, and performs real-time entity lookup.
    /// </summary>
    [HttpPost("lookup")]
    [RequirePermission(PermissionCodes.QrCode.Scan)]
    public async Task<ActionResult<ApiResponse<QrScanResultDto>>> Lookup([FromBody] QrScanRequest request, CancellationToken ct)
    {
        var result = await _service.ScanAndLookupAsync(request.Payload, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<QrScanResultDto>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    /// <summary>
    /// Fetches list of available entities (PRODUCT, LOCATION, MACHINE, EMPLOYEE, GOODS_RECEIPT, INVENTORY) for QR picker.
    /// </summary>
    [HttpGet("available-items/{entityType}")]
    [RequirePermission(PermissionCodes.QrCode.View)]
    public async Task<ActionResult<ApiResponse<List<QrItemLookupDto>>>> GetAvailableItems(string entityType, CancellationToken ct)
    {
        var result = await _service.GetAvailableItemsAsync(entityType, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<List<QrItemLookupDto>>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }

    /// <summary>
    /// Generates QR codes for multiple items for batch label printing.
    /// </summary>
    [HttpPost("batch-generate")]
    [RequirePermission(PermissionCodes.QrCode.Generate)]
    public async Task<ActionResult<ApiResponse<List<QrCodeDto>>>> BatchGenerate([FromBody] QrBatchPrintRequest request, CancellationToken ct)
    {
        var result = await _service.BatchGenerateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Failure<List<QrCodeDto>>(result.Error, HttpStatusCode.BadRequest));
        }
        return Ok(ApiResponseFactory.Success(result.Data));
    }
}
