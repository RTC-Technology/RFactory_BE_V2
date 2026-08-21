using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Application.Modules.Inventory.Services;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.GoodsReceipt.Services;

/// <summary>
/// A goods receipt is only meaningful together with its lines, so every write here covers
/// both and runs inside one transaction: a failed line must not leave a receipt behind.
/// </summary>
public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly IRepository<Entities.GoodsReceipt> _goodsReceipt;
    private readonly IRepository<Entities.GoodsReceiptDetail> _goodsReceiptDetail;
    private readonly IRepository<Entities.Inventory> _inventory;
    private readonly IRepository<Entities.InventoryTransaction> _transaction;
    private readonly IInventoryTransactionService _transactionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GoodsReceiptService(
        IRepository<Entities.GoodsReceipt> goodsReceipt,
        IRepository<Entities.GoodsReceiptDetail> goodsReceiptDetail,
        IRepository<Entities.Inventory> inventory,
        IRepository<Entities.InventoryTransaction> transaction,
        IInventoryTransactionService transactionService,
    IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _goodsReceipt = goodsReceipt;
        _goodsReceiptDetail = goodsReceiptDetail;
        _inventory = inventory;
        _transaction = transaction;
        _transactionService = transactionService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GoodsReceiptDto>> CreateAsync(CreateGoodsReceiptRequest request, CancellationToken ct = default)
    {
        var existing = await _goodsReceipt.FirstOrDefault(t => t.ReceiptNo == request.ReceiptNo, ct);
        if (existing is not null)
        {
            return Result<GoodsReceiptDto>.Failure($"Goods Receipt '{request.ReceiptNo}' already exists.");
        }

        var entity = _mapper.Map<Entities.GoodsReceipt>(request);
        var lines = request.GoodsReceiptDetails ?? new List<GoodsReceiptLineRequest>();

        return await _unitOfWork.ExecuteAsync(async token =>
        {
            // Two saves rather than one: the lines need the id the database generates for
            // the receipt, which is only known once the receipt is in.
            await _goodsReceipt.Add(entity, token);
            await _goodsReceiptDetail.AddRange(
                lines.Select(line => ToLineEntity(line, entity.Id)).ToList(), token);

            //Add inventory
            var inventories = await AddInventoryAsync(lines, token);
            if (inventories.Count > 0) await _inventory.AddRange(inventories, token);

            //Add inventory transaction
            var newTransactionLines = lines.Select(x => new CreateInventoryTransactionRequest
            {
                Id = x.Id,
                ProductId = (long)x.ProductId,
                WarehouseId = (long)entity.WarehouseId,
                WarehouseLocationId = (long)(x.LocationId ?? 0),
                Quantity = x.Quantity,
                UnitId = (long)x.UnitId
            }).ToList();
            var transactionChanges = _transactionService.BuildTransactionChanges([], newTransactionLines, (line, action) => ToTransactionEntity(line, action, entity));
            if (transactionChanges.Count > 0) await _transaction.AddRange(transactionChanges, token);


            return Result<GoodsReceiptDto>.Success(_mapper.Map<GoodsReceiptDto>(entity));
        }, ct);
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _goodsReceipt.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Goods Receipt {id} was not found.");
        }

        var receiptId = (long)id;
        var lines = await _goodsReceiptDetail.Where(p => p.GoodsReceiptId == receiptId, ct);

        // The lines belong to this receipt and nothing else, so they go with it instead of
        // blocking the delete — deleting is soft on both, and the pair moves together.
        return await _unitOfWork.ExecuteAsync<Result>(async token =>
        {
            await _goodsReceiptDetail.DeleteRange(lines, token);
            await _goodsReceipt.Delete(entity, token);

            //Add inventory transaction
            var oldLines = lines.Select(x => new CreateInventoryTransactionRequest
            {
                Id = x.Id,
                ProductId = x.ProductId,
                WarehouseId = entity.WarehouseId,
                WarehouseLocationId = x.LocationId,
                Quantity = x.Quantity,
                UnitId = x.UnitId
            }).ToList();
            

            var transactionChanges = _transactionService.BuildTransactionChanges(oldLines, [], (line, action) => ToTransactionEntity(line, action, entity));
            if (transactionChanges.Count > 0) await _transaction.AddRange(transactionChanges, token);

            return Result.Success();
        }, ct);
    }

    public async Task<List<GoodsReceiptDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<GoodsReceiptDto>>(await _goodsReceipt.GetAll(ct));

    public async Task<GoodsReceiptDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _goodsReceipt.GetById(id, ct);
        return entity is null ? null : _mapper.Map<GoodsReceiptDto>(entity);
    }

    public async Task<Result<GoodsReceiptDto>> UpdateAsync(ulong id, UpdateGoodsReceiptRequest request, CancellationToken ct = default)
    {
        var entity = await _goodsReceipt.GetById(id, ct);
        if (entity is null)
        {
            return Result<GoodsReceiptDto>.Failure($"Goods Receipt {id} was not found.");
        }

        var existing = await _goodsReceipt.FirstOrDefault(
            t => t.Id != id && t.ReceiptNo == request.ReceiptNo, ct);
        if (existing is not null)
        {
            return Result<GoodsReceiptDto>.Failure($"Goods Receipt '{request.ReceiptNo}' already exists.");
        }

        var receiptId = (long)id;
        var stored = await _goodsReceiptDetail.Where(l => l.GoodsReceiptId == receiptId, ct);

        var lines = request.GoodsReceiptDetails;
        var keptIds = (lines ?? new List<GoodsReceiptLineRequest>())
            .Where(line => line.Id != 0)
            .Select(line => line.Id)
            .ToHashSet();

        // The list replaces the whole set, so an id from another receipt would be edited
        // here and dropped from where it belongs. Reject the payload instead.
        var foreign = keptIds.Where(lineId => stored.All(s => s.Id != lineId)).ToList();
        if (foreign.Count > 0)
        {
            return Result<GoodsReceiptDto>.Failure(
                $"Line(s) {string.Join(", ", foreign)} do not belong to Goods Receipt {id}.");
        }

        var oldReceipt = new Entities.GoodsReceipt
        {
            Id = entity.Id,
            ReceiptNo = entity.ReceiptNo,
            WarehouseId = entity.WarehouseId
        };

        _mapper.Map(request, entity);

        return await _unitOfWork.ExecuteAsync(async token =>
        {
            await _goodsReceipt.Update(entity, token);

            // A null list means the caller is editing the header only; an empty one means
            // the receipt really has no lines left.
            if (lines is not null)
            {

                var newLines = lines.ToList();
                var oldLines = stored.Select(x => new CreateInventoryTransactionRequest
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    WarehouseId = oldReceipt.WarehouseId,
                    WarehouseLocationId = x.LocationId,
                    Quantity = x.Quantity,
                    UnitId = x.UnitId
                }).ToList();

                var newTransactionLines = lines.Select(x => new CreateInventoryTransactionRequest
                {
                    Id = x.Id,
                    ProductId = (long)(x.ProductId),
                    WarehouseId = (long)(entity.WarehouseId),
                    WarehouseLocationId = (long)(x.LocationId ?? 0),
                    Quantity = x.Quantity,
                    UnitId = (long)(x.UnitId)
                }).ToList();

                var transactionChanges = _transactionService.BuildTransactionChanges(oldLines, newTransactionLines, (line, action) => ToTransactionEntity(line, action, entity));


                await _goodsReceiptDetail.DeleteRange(
                    stored.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                foreach (var line in lines.Where(l => l.Id != 0))
                {
                    var target = stored.First(s => s.Id == line.Id);
                    _mapper.Map(line, target);
                    await _goodsReceiptDetail.Update(target, token);
                }

                await _goodsReceiptDetail.AddRange(
                    lines.Where(l => l.Id == 0).Select(line => ToLineEntity(line, id)).ToList(), token);

                //Add inventory
                var inventories = await AddInventoryAsync(lines, token);
                if (inventories.Count > 0) await _inventory.AddRange(inventories, token);

                //Add inventory transaction
                if (transactionChanges.Count > 0) await _transaction.AddRange(transactionChanges, token);

            }

            return Result<GoodsReceiptDto>.Success(_mapper.Map<GoodsReceiptDto>(entity));
        }, ct);
    }

    private Entities.GoodsReceiptDetail ToLineEntity(GoodsReceiptLineRequest line, ulong receiptId)
    {
        var entity = _mapper.Map<Entities.GoodsReceiptDetail>(line);
        entity.GoodsReceiptId = (long)receiptId;
        return entity;
    }

    private Entities.Inventory ToInventoryEntity(GoodsReceiptLineRequest line)
    {
        var entity = new Entities.Inventory
        {
            ProductId = (long)line.ProductId,
            LocationId = (long)(line.LocationId ?? 0),
            //LotNo = line.LotNo,
            //SerialNo = line.SerialNo,
            //Quantity = line.Quantity,
            //ReservedQuantity = 0,
            //AvailableQuantity = 0,
            UnitId = (long)line.UnitId,
            LastTransactionDate = DateTime.Now
        };

        return entity;
    }


    private async Task<List<Entities.Inventory>> AddInventoryAsync(List<GoodsReceiptLineRequest> lines, CancellationToken ct = default)
    {
        var inventories = new List<Entities.Inventory>();

        foreach (var line in lines)
        {
            var locationId = (long)(line.LocationId ?? 0);

            var existing = await _inventory.FirstOrDefault(x => x.ProductId == (long)line.ProductId && x.LocationId == locationId, ct);

            if (existing is null)
            {
                inventories.Add(ToInventoryEntity(line));
            }
        }

        return inventories;
    }

    private Entities.InventoryTransaction ToTransactionEntity(CreateInventoryTransactionRequest line, InventoryTransactionActionType action, Entities.GoodsReceipt entity)
    {
        return new Entities.InventoryTransaction
        {
            TransactionNo = entity.ReceiptNo,
            TransactionType = (int)InventoryTransactionType.Receipt,
            ReferenceType = (int)InventoryReferenceType.GoodsReceipt,
            ReferenceId = (long)entity.Id,

            ProductId = line.ProductId,
            WarehouseId = line.WarehouseId,
            WarehouseLocationId = line.WarehouseLocationId,
            Quantity = line.Quantity,
            UnitId = line.UnitId,

            ActionType = (int)action,
            TransactionDate = DateTime.UtcNow
        };
    }
}
