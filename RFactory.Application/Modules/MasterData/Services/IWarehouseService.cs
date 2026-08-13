using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.MasterData.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehouseDto>> GetAllAsync(CancellationToken ct = default);
        Task<WarehouseDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    }

    public interface IWarehouseLocationService
    {
        Task<List<WarehouseLocationDto>> GetAllAsync(CancellationToken ct = default);
        Task<WarehouseLocationDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        //Task<Result<WarehouseLocationDto>> CreateAsync(CreateAreaRequest request, CancellationToken ct = default);
        //Task<Result<WarehouseLocationDto>> UpdateAsync(ulong id, UpdateAreaRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
