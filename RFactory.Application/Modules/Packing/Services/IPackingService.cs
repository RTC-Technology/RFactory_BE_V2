using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Packing.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Packing.Services
{
    public interface IPackingCheckService
    {
        Task<List<PackingCheckDto>> GetAllAsync(CancellationToken ct = default);
        Task<PackingCheckDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PackingCheckDto>> CreateAsync(PackingCheckRequest request, CancellationToken ct = default);
        Task<Result<PackingCheckDto>> UpdateAsync(ulong id, PackingCheckRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IPackingCheckItemService
    {
        Task<List<PackingCheckItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<PackingCheckItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PackingCheckItemDto>> CreateAsync(PackingCheckItemRequest request, CancellationToken ct = default);
        Task<Result<PackingCheckItemDto>> UpdateAsync(ulong id, PackingCheckItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IPackingPackageService
    {
        Task<List<PackingPackageDto>> GetAllAsync(CancellationToken ct = default);
        Task<PackingPackageDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PackingPackageDto>> CreateAsync(PackingPackageRequest request, CancellationToken ct = default);
        Task<Result<PackingPackageDto>> UpdateAsync(ulong id, PackingPackageRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IPackingPackageItemService
    {
        Task<List<PackingPackageItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<PackingPackageItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PackingPackageItemDto>> CreateAsync(PackingPackageItemRequest request, CancellationToken ct = default);
        Task<Result<PackingPackageItemDto>> UpdateAsync(ulong id, PackingPackageItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IPackingScanLogService
    {
        Task<List<PackingScanLogDto>> GetAllAsync(CancellationToken ct = default);
        Task<PackingScanLogDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<PackingScanLogDto>> CreateAsync(PackingScanLogRequest request, CancellationToken ct = default);
        Task<Result<PackingScanLogDto>> UpdateAsync(ulong id, PackingScanLogRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
