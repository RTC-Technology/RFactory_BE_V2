using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ICurrencyService
    {
        Task<List<CurrencyDto>> GetAllAsync(CancellationToken ct = default);
        Task<CurrencyDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<CurrencyDto>> CreateAsync(CurrencyRequest request, CancellationToken ct = default);
        Task<Result<CurrencyDto>> UpdateAsync(ulong id, CurrencyRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
