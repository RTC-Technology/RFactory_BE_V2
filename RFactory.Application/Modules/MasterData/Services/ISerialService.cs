using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ISerialService
    {
        Task<List<SerialDto>> GetAllAsync(CancellationToken ct = default);
        Task<SerialDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SerialDto>> CreateAsync(SerialRequest request, CancellationToken ct = default);
        Task<Result<SerialDto>> UpdateAsync(ulong id, SerialRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ISerialRuleService
    {
        Task<List<SerialRuleDto>> GetAllAsync(CancellationToken ct = default);
        Task<SerialRuleDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SerialRuleDto>> CreateAsync(SerialRuleRequest request, CancellationToken ct = default);
        Task<Result<SerialRuleDto>> UpdateAsync(ulong id, SerialRuleRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface ISerialRuleSequenceService
    {
        Task<List<SerialRuleSequenceDto>> GetAllAsync(CancellationToken ct = default);
        Task<SerialRuleSequenceDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SerialRuleSequenceDto>> CreateAsync(SerialRuleSequenceRequest request, CancellationToken ct = default);
        Task<Result<SerialRuleSequenceDto>> UpdateAsync(ulong id, SerialRuleSequenceRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
