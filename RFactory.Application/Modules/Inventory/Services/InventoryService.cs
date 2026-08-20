using AutoMapper;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Infrastructure.Dapper;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities = RFactory.Infrastructure.Entities;
namespace RFactory.Application.Modules.Inventory.Services;

public class InventoryService : IInventoryService
{
    private readonly IRepository<Entities.Inventory> _repository;
    private readonly IMapper _mapper;
    private readonly IProcedureExecutor _proc;

    public InventoryService(IRepository<Entities.Inventory> repository, IMapper mapper, IProcedureExecutor proc)
    {
        _repository = repository;
        _mapper = mapper;
        _proc = proc;
    }

    public async Task<Result<InventoryDto>> CreateAsync(CreateInventoryRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.Inventory>(request);
        await _repository.Add(entity, ct);
        return Result<InventoryDto>.Success(_mapper.Map<InventoryDto>(entity));
    }


    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Inventory {id} was not found.");
    }

    /// <summary>Lines of one receipt, or every line when <paramref name="receiptId"/> is null.</summary>
    public async Task<List<InventoryDto>> GetAllAsync(CancellationToken ct = default)
    //=> _mapper.Map<List<InventoryDto>>(await _repository.GetAll(ct));
    {
        var param = new{};
        var entities = await _proc.QueryAsync<InventoryDto>("spGetInventory", param);
        return entities;
    }


    public async Task<InventoryDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<InventoryDto>(entity);
    }

    public async Task<Result<InventoryDto>> UpdateAsync(ulong id, UpdateInventoryRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<InventoryDto>.Failure($"Inventory {id} was not found.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<InventoryDto>.Success(_mapper.Map<InventoryDto>(entity));
    }
}


public class InventoryTransactionService : IInventoryTransactionService
{
    private readonly IRepository<Entities.InventoryTransaction> _repository;
    private readonly IMapper _mapper;
    private readonly IProcedureExecutor _proc;

    public InventoryTransactionService(IRepository<Entities.InventoryTransaction> repository, IMapper mapper, IProcedureExecutor proc)
    {
        _repository = repository;
        _mapper = mapper;
        _proc = proc;
    }

    public async Task<Result<InventoryTransactionDto>> CreateAsync(CreateInventoryTransactionRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.InventoryTransaction>(request);
        await _repository.Add(entity, ct);
        return Result<InventoryTransactionDto>.Success(_mapper.Map<InventoryTransactionDto>(entity));
    }


    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Inventory transaction {id} was not found.");
    }

    /// <summary>Lines of one receipt, or every line when <paramref name="receiptId"/> is null.</summary>
    public async Task<List<InventoryTransactionDto>> GetAllAsync(CancellationToken ct = default)
    => _mapper.Map<List<InventoryTransactionDto>>(await _repository.GetAll(ct));



    public async Task<InventoryTransactionDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<InventoryTransactionDto>(entity);
    }

    public async Task<Result<InventoryTransactionDto>> UpdateAsync(ulong id, UpdateInventoryTransactionRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<InventoryTransactionDto>.Failure($"Inventory transaction {id} was not found.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<InventoryTransactionDto>.Success(_mapper.Map<InventoryTransactionDto>(entity));
    }

    public List<InventoryTransaction> BuildTransactionChanges(IEnumerable<CreateInventoryTransactionRequest> oldLines, IEnumerable<CreateInventoryTransactionRequest> newLines, Func<CreateInventoryTransactionRequest, InventoryTransactionActionType, InventoryTransaction> createTransaction, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var oldById = oldLines.ToDictionary(x => x.Id);
        var newList = newLines.ToList();

        var transactions = new List<InventoryTransaction>();

        // REMOVE
        foreach (var oldLine in oldById.Values)
        {
            ct.ThrowIfCancellationRequested();

            if (!newList.Any(x => x.Id == oldLine.Id))
            {
                transactions.Add(createTransaction(oldLine, InventoryTransactionActionType.Remove));
            }
        }

        // UPDATE / REPLACE
        foreach (var newLine in newList.Where(x => x.Id != 0))
        {
            ct.ThrowIfCancellationRequested();

            if (!oldById.TryGetValue(newLine.Id, out var oldLine)) continue;

            // Product hoặc Warehouse thay đổi
            if (oldLine.ProductId != newLine.ProductId || oldLine.WarehouseId != newLine.WarehouseId)
            {
                transactions.Add(createTransaction(oldLine, InventoryTransactionActionType.Remove));
                transactions.Add(createTransaction(newLine, InventoryTransactionActionType.Add));
                continue;
            }

            // Cùng Product + Warehouse nhưng thông tin line thay đổi
            if (oldLine.WarehouseLocationId != newLine.WarehouseLocationId ||
                oldLine.Quantity != newLine.Quantity ||
                oldLine.UnitId != newLine.UnitId
                )
            {
                transactions.Add(createTransaction(newLine, InventoryTransactionActionType.Update));
            }
        }

        // ADD
        foreach (var newLine in newList.Where(x => x.Id == 0))
        {
            ct.ThrowIfCancellationRequested();
            transactions.Add(createTransaction(newLine,InventoryTransactionActionType.Add));
        }

        return transactions;
    }
}