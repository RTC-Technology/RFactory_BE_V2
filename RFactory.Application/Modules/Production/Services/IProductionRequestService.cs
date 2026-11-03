using RFactory.Application.Modules.Production.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Production.Services
{
    public interface IProductionRequestService
    {
        Task<List<ProductionRequestDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductionRequestDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<ProductionRequestDto>> CreateAsync(ProductionRequestRequest request, CancellationToken ct = default);
        Task<Result<ProductionRequestDto>> UpdateAsync(ulong id, ProductionRequestRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
    public interface IProductionRequestItemService
    {
        Task<List<ProductionRequestItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductionRequestItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
        Task<Result<ProductionRequestItemDto>> CreateAsync(ProductionRequestItemRequest request, CancellationToken ct = default);
        Task<Result<ProductionRequestItemDto>> UpdateAsync(ulong id, ProductionRequestItemRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
    }
}
