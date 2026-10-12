using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface IFailureCodeService
    {
        Task<List<FailureCodeDto>> GetAllAsync(CancellationToken ct = default);
        Task<FailureCodeDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<FailureCodeDto>> CreateAsync(FailureCodeRequest request, CancellationToken ct = default);
        Task<Result<FailureCodeDto>> UpdateAsync(ulong id, FailureCodeRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }

    public interface IFailureGroupService
    {
        Task<List<FailureGroupDto>> GetAllAsync(CancellationToken ct = default);
        Task<FailureGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<FailureGroupDto>> CreateAsync(FailureGroupRequest request, CancellationToken ct = default);
        Task<Result<FailureGroupDto>> UpdateAsync(ulong id, FailureGroupRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
