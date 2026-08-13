using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync(CancellationToken ct = default);
        Task<SupplierDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<SupplierDto>> CreateAsync(CreateSupplierRequest request, CancellationToken ct = default);
        Task<Result<SupplierDto>> UpdateAsync(ulong id, UpdateSupplierRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
