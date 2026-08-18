using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFactory.Application.Modules.Inventory.Services;

public interface IInventoryService
{
    Task<List<InventoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<InventoryDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<InventoryDto>> CreateAsync(CreateInventoryRequest request, CancellationToken ct = default);
    Task<Result<InventoryDto>> UpdateAsync(ulong id, UpdateInventoryRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}

public interface IInventoryTransactionService
{
    Task<List<InventoryTransactionDto>> GetAllAsync(CancellationToken ct = default);
    Task<InventoryTransactionDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<InventoryTransactionDto>> CreateAsync(CreateInventoryTransactionRequest request, CancellationToken ct = default);
    Task<Result<InventoryTransactionDto>> UpdateAsync(ulong id, UpdateInventoryTransactionRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
