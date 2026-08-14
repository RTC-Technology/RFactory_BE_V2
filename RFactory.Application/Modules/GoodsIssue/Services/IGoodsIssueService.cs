using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.GoodsIssue.Services;

public interface IGoodsIssueService
{
    Task<List<GoodsIssueDto>> GetAllAsync(CancellationToken ct = default);
    Task<GoodsIssueDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<GoodsIssueDto>> CreateAsync(CreateGoodsIssueRequest request, CancellationToken ct = default);
    Task<Result<GoodsIssueDto>> UpdateAsync(ulong id, UpdateGoodsIssueRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}


public interface IGoodsIssueDetailService
{
    Task<List<GoodsIssueDetailDto>> GetAllAsync(long? issueId,CancellationToken ct = default);
    Task<GoodsIssueDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<GoodsIssueDetailDto>> CreateAsync(GoodsIssueDetailRequest request, CancellationToken ct = default);
    Task<Result<GoodsIssueDetailDto>> UpdateAsync(ulong id, GoodsIssueDetailRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
