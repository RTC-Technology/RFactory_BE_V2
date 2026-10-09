using AutoMapper;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Application.Modules.PurchaseOrder.Services;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.PickingPlan.Services
{
    public class PickingPlanService : IPickingPlanService
    {
        private readonly IRepository<Entities.PickingPlan> _repository;
        private readonly IRepository<Entities.PickingPlanSource> _source;
        private readonly IRepository<Entities.PickingPlanItem> _item;
        private readonly IRepository<Entities.PickingPlanItemSource> _itemSource;
        private readonly IRepository<Entities.PickingTicket> _ticket;
        private readonly IRepository<Entities.PickingTicketItem> _ticketItem;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PickingPlanService(
            IRepository<Entities.PickingPlan> repository,
            IRepository<Entities.PickingPlanSource> source,
            IRepository<Entities.PickingPlanItem> item,
            IRepository<Entities.PickingPlanItemSource> itemSource,
            IRepository<Entities.PickingTicket> ticket,
            IRepository<Entities.PickingTicketItem> ticketItem,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _source = source;
            _item = item;
            _itemSource = itemSource;
            _ticket = ticket;
            _ticketItem = ticketItem;

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PickingPlanDto>> CreateAsync(PickingPlanRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(t => t.PlanNo == request.PlanNo, ct);

            if (existing is not null)
            {
                return Result<PickingPlanDto>.Failure($"Picking plan '{request.PlanNo}' already exists.");
            }

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // =========================================================
                // 1. INSERT PICKING PLAN
                // =========================================================

                var entity = _mapper.Map<Entities.PickingPlan>(request);

                // Không để AutoMapper map các collection xuống entity
                //entity.PickingPlanItems = new List<Entities.PickingPlanItem>();
                //entity.PickingPlanSources = new List<Entities.PickingPlanSource>();
                //entity.PickingTickets = new List<Entities.PickingTicket>();

                await _repository.Add(entity, token);

                // =========================================================
                // 2. INSERT SOURCES
                // =========================================================

                var sources = request.PickingPlanSources ?? new List<PickingPlanSourceRequest>();

                if (sources.Count > 0)
                {
                    var sourceEntities = sources.Select(x => ToSourceEntity(x, entity.Id)).ToList();

                    await _source.AddRange(sourceEntities, token);
                }

                // =========================================================
                // 3. INSERT PLAN ITEMS
                // =========================================================

                var items = request.PickingPlanItems ?? new List<PickingPlanItemRequest>();

                // Map:
                // client uId -> database PickingPlanItem.Id
                var itemIdMap = new Dictionary<long, ulong>();

                foreach (var itemRequest in items)
                {
                    var itemEntity = ToPlanItemEntity(itemRequest, entity.Id);

                    await _item.Add(itemEntity, token);

                    // Sau khi Add, EF phải có entity.Id
                    if (itemRequest.UId.HasValue)
                    {
                        itemIdMap[itemRequest.UId.Value] = itemEntity.Id;
                    }

                    // =====================================================
                    // 3.1 INSERT PLAN ITEM SOURCES
                    // =====================================================

                    var itemSources = itemRequest.PickingPlanItemSources ?? new List<PickingPlanItemSourceRequest>();

                    if (itemSources.Count > 0)
                    {
                        var itemSourceEntities = itemSources.Select(x => ToPlanItemSourceEntity(x, itemEntity.Id)).ToList();
                        await _itemSource.AddRange(itemSourceEntities, token);
                    }
                }

                // =========================================================
                // 4. INSERT PICKING TICKETS
                // =========================================================

                var tickets = request.PickingTickets ?? new List<PickingTicketRequest>();

                foreach (var ticketRequest in tickets)
                {
                    var ticketEntity = ToTicketEntity(ticketRequest, entity.Id);

                    await _ticket.Add(ticketEntity, token);

                    // =====================================================
                    // 4.1 INSERT TICKET ITEMS
                    // =====================================================

                    var ticketItems = ticketRequest.PickingTicketItems ?? new List<PickingTicketItemRequest>();

                    foreach (var ticketItemRequest in ticketItems)
                    {
                        ulong? pickingPlanItemId = null;

                        // pickingPlanItemId hiện tại của client
                        // chính là UId của PickingPlanItem
                        if (ticketItemRequest.PickingPlanItemId.HasValue)
                        {
                            if (!itemIdMap.TryGetValue(ticketItemRequest.PickingPlanItemId.Value, out var realPlanItemId))
                            {
                                throw new InvalidOperationException($"PickingPlanItem with client key " + $"{ticketItemRequest.PickingPlanItemId} not found.");
                            }

                            pickingPlanItemId = realPlanItemId;
                        }

                        var ticketItemEntity = ToTicketItemEntity(ticketItemRequest, ticketEntity.Id, pickingPlanItemId);

                        await _ticketItem.Add(ticketItemEntity, token);
                    }
                }

                // =========================================================
                // 5. RETURN
                // =========================================================

                return Result<PickingPlanDto>.Success(
                    _mapper.Map<PickingPlanDto>(entity));

            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Picking plan {id} was not found.");
            }

            var pickingId = (long)id;
            var sources = await _source.Where(p => p.PickingPlanId == pickingId, ct);
            var items = await _item.Where(x => x.PickingPlanId == pickingId, ct);
            var itemSources = await _itemSource.Where(x => items.Select(i => i.Id).Contains((ulong)(x.PickingPlanItemId ?? 0)), ct);
            var tickets = await _ticket.Where(x => x.PickingPlanId == pickingId, ct);
            var ticketItems = await _ticketItem.Where(x => tickets.Select(t => t.Id).Contains((ulong)(x.PickingTicketId ?? 0)), ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {

                await _itemSource.DeleteRange(itemSources, token);
                await _ticketItem.DeleteRange(ticketItems, token);
                await _source.DeleteRange(sources, token);
                await _item.DeleteRange(items, token);
                await _ticket.DeleteRange(tickets, token);
                await _repository.Delete(entity, token);

                return Result.Success();
            }, ct);
        }

        public async Task<List<PickingPlanDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<PickingPlanDto>>(await _repository.GetAll(ct));

        public async Task<PickingPlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PickingPlanDto>(entity);
        }

        public async Task<Result<PickingPlanDto>> UpdateAsync(ulong id, PickingPlanRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);

            if (entity is null)
            {
                return Result<PickingPlanDto>.Failure($"Picking plan {id} was not found.");
            }

            var pickingId = (long)id;
            var storedItems = await _item.Where(x => x.PickingPlanId == pickingId, ct);
            var storedSources = await _source.Where(x => x.PickingPlanId == pickingId, ct);
            var storedTickets = await _ticket.Where(x => x.PickingPlanId == pickingId, ct);
            
            var items = request.PickingPlanItems;
            var sources = request.PickingPlanSources;
            var tickets = request.PickingTickets;

            // =========================================================
            // VALIDATE PLAN ITEMS
            // =========================================================

            if (items is not null)
            {
                var keptItemIds = items
                    .Where(x => x.Id != 0)
                    .Select(x => x.Id)
                    .ToHashSet();

                var foreignItemIds = keptItemIds
                    .Where(itemId => storedItems.All(x => x.Id != itemId))
                    .ToList();

                if (foreignItemIds.Count > 0)
                {
                    return Result<PickingPlanDto>.Failure($"Item(s) {string.Join(", ", foreignItemIds)} " + $"do not belong to Picking plan {id}.");
                }
            }

            // =========================================================
            // VALIDATE PLAN SOURCES
            // =========================================================

            if (sources is not null)
            {
                var keptSourceIds = sources
                    .Where(x => x.Id != 0)
                    .Select(x => x.Id)
                    .ToHashSet();

                var foreignSourceIds = keptSourceIds
                    .Where(sourceId => storedSources.All(x => x.Id != sourceId))
                    .ToList();

                if (foreignSourceIds.Count > 0)
                {
                    return Result<PickingPlanDto>.Failure($"Source(s) {string.Join(", ", foreignSourceIds)} " + $"do not belong to Picking plan {id}.");
                }
            }

            // =========================================================
            // VALIDATE TICKETS
            // =========================================================

            if (tickets is not null)
            {
                var keptTicketIds = tickets
                    .Where(x => x.Id != 0)
                    .Select(x => x.Id)
                    .ToHashSet();

                var foreignTicketIds = keptTicketIds
                    .Where(ticketId => storedTickets.All(x => x.Id != ticketId))
                    .ToList();

                if (foreignTicketIds.Count > 0)
                {
                    return Result<PickingPlanDto>.Failure($"Ticket(s) {string.Join(", ", foreignTicketIds)} " + $"do not belong to Picking plan {id}.");
                }
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // =====================================================
                // UPDATE PICKING PLAN
                // =====================================================

                await _repository.Update(entity, token);

                // uId/client temporary key -> real DB PickingPlanItem.Id
                //
                // Example:
                // uId = 1  -> DB Id = 1
                // uId = -1 -> DB Id = 25
                //
                var itemIdMap = new Dictionary<long, ulong>();

                // =====================================================
                // PICKING PLAN ITEMS
                // =====================================================

                if (items is not null)
                {
                    var keptItemIds = items
                        .Where(x => x.Id != 0)
                        .Select(x => x.Id)
                        .ToHashSet();

                    // -------------------------------------------------
                    // DELETE ITEMS NOT IN REQUEST
                    // -------------------------------------------------

                    var removedItems = storedItems
                        .Where(x => !keptItemIds.Contains(x.Id))
                        .ToList();

                    if (removedItems.Count > 0)
                    {
                        var removedItemIds = removedItems
                            .Select(x => x.Id)
                            .ToHashSet();

                        // Delete Item Sources
                        var removedItemSources = await _itemSource.Where(x => removedItemIds.Contains((ulong)(x.PickingPlanItemId ?? 0)), token);

                        if (removedItemSources.Count > 0)
                        {
                            await _itemSource.DeleteRange(removedItemSources, token);
                        }

                        // Delete Ticket Items referencing these Plan Items
                        var removedTicketItems = await _ticketItem
                                                .Where(x => x.PickingPlanItemId.HasValue && removedItemIds.Contains((ulong)x.PickingPlanItemId.Value), token);

                        if (removedTicketItems.Count > 0)
                        {
                            await _ticketItem.DeleteRange(removedTicketItems, token);
                        }

                        // Delete Plan Items
                        await _item.DeleteRange(removedItems, token);
                    }

                    // -------------------------------------------------
                    // UPDATE EXISTING ITEMS
                    // -------------------------------------------------

                    foreach (var itemRequest in items.Where(x => x.Id != 0))
                    {
                        var target = storedItems.First(x => x.Id == itemRequest.Id);

                        _mapper.Map(itemRequest, target);

                        await _item.Update(target, token);

                        // ---------------------------------------------
                        // Map uId -> DB Id
                        // ---------------------------------------------

                        if (itemRequest.UId.HasValue)
                        {
                            itemIdMap[itemRequest.UId.Value] = target.Id;
                        }

                        // ---------------------------------------------
                        // Item Sources
                        //
                        // Nếu pickingPlanItemSources = null
                        // => giữ nguyên
                        //
                        // Nếu [] hoặc có dữ liệu
                        // => replace toàn bộ
                        // ---------------------------------------------

                        if (itemRequest.PickingPlanItemSources is not null)
                        {
                            var oldSources = await _itemSource.Where(x => x.PickingPlanItemId == (long)target.Id, token);

                            if (oldSources.Count > 0)
                            {
                                await _itemSource.DeleteRange(oldSources, token);
                            }

                            var newSources = itemRequest.PickingPlanItemSources
                                .Select(x => ToPlanItemSourceEntity(x, target.Id))
                                .ToList();

                            if (newSources.Count > 0)
                            {
                                await _itemSource.AddRange(newSources, token);
                            }
                        }
                    }

                    // -------------------------------------------------
                    // INSERT NEW ITEMS
                    // -------------------------------------------------

                    foreach (var itemRequest in items.Where(x => x.Id == 0))
                    {
                        var newItem = ToPlanItemEntity(itemRequest, id);

                        await _item.Add(newItem, token);

                        // ---------------------------------------------
                        // QUAN TRỌNG:
                        //
                        // uId = -1
                        // DB Id = 25
                        //
                        // itemIdMap[-1] = 25
                        // ---------------------------------------------

                        if (itemRequest.UId.HasValue)
                        {
                            itemIdMap[itemRequest.UId.Value] = newItem.Id;
                        }

                        // ---------------------------------------------
                        // Insert Item Sources
                        // ---------------------------------------------

                        if (itemRequest.PickingPlanItemSources is not null)
                        {
                            var newSources = itemRequest.PickingPlanItemSources
                                .Select(x => ToPlanItemSourceEntity(x, newItem.Id))
                                .ToList();

                            if (newSources.Count > 0)
                            {
                                await _itemSource.AddRange(newSources, token);
                            }
                        }
                    }
                }

                // =====================================================
                // PICKING PLAN SOURCES
                // =====================================================

                if (sources is not null)
                {
                    var keptSourceIds = sources
                        .Where(x => x.Id != 0)
                        .Select(x => x.Id)
                        .ToHashSet();

                    // -------------------------------------------------
                    // DELETE
                    // -------------------------------------------------

                    var removedSources = storedSources
                        .Where(x => !keptSourceIds.Contains(x.Id))
                        .ToList();

                    if (removedSources.Count > 0)
                    {
                        await _source.DeleteRange(removedSources, token);
                    }

                    // -------------------------------------------------
                    // UPDATE
                    // -------------------------------------------------

                    foreach (var sourceRequest in sources.Where(x => x.Id != 0))
                    {
                        var target = storedSources.First(x => x.Id == sourceRequest.Id);

                        _mapper.Map(sourceRequest, target);

                        await _source.Update(target, token);
                    }

                    // -------------------------------------------------
                    // INSERT
                    // -------------------------------------------------

                    var newSources = sources
                        .Where(x => x.Id == 0)
                        .Select(x => ToSourceEntity(x, id))
                        .ToList();

                    if (newSources.Count > 0)
                    {
                        await _source.AddRange(newSources, token);
                    }
                }

                // =====================================================
                // PICKING TICKETS
                // =====================================================

                if (tickets is not null)
                {
                    var keptTicketIds = tickets
                        .Where(x => x.Id != 0)
                        .Select(x => x.Id)
                        .ToHashSet();

                    // -------------------------------------------------
                    // DELETE TICKETS NOT IN REQUEST
                    // -------------------------------------------------

                    var removedTickets = storedTickets
                        .Where(x => !keptTicketIds.Contains(x.Id))
                        .ToList();

                    if (removedTickets.Count > 0)
                    {
                        var removedTicketIds = removedTickets
                            .Select(x => x.Id)
                            .ToHashSet();

                        // Delete Ticket Items first
                        var removedTicketItems = await _ticketItem.Where(x => removedTicketIds.Contains((ulong)(x.PickingTicketId ?? 0)), token);

                        if (removedTicketItems.Count > 0)
                        {
                            await _ticketItem.DeleteRange(removedTicketItems, token);
                        }

                        // Then Ticket
                        await _ticket.DeleteRange(removedTickets, token);
                    }

                    // -------------------------------------------------
                    // UPDATE EXISTING TICKETS
                    // -------------------------------------------------

                    foreach (var ticketRequest in tickets.Where(x => x.Id != 0))
                    {
                        var target = storedTickets.First(x => x.Id == ticketRequest.Id);

                        _mapper.Map(ticketRequest, target);
                        await _ticket.Update(target, token);

                        // ---------------------------------------------
                        // Ticket Items = null
                        // => giữ nguyên
                        // ---------------------------------------------

                        if (ticketRequest.PickingTicketItems is null) continue;


                        var ticketItems = ticketRequest.PickingTicketItems;

                        var storedTicketItems = await _ticketItem.Where(x => x.PickingTicketId == (long)target.Id, token);

                        // ---------------------------------------------
                        // Validate Ticket Item IDs
                        // ---------------------------------------------

                        var keptTicketItemIds = ticketItems
                            .Where(x => x.Id != 0)
                            .Select(x => x.Id)
                            .ToHashSet();

                        var storedTicketItemIds = storedTicketItems
                            .Select(x => x.Id)
                            .ToHashSet();

                        var foreignTicketItemIds = keptTicketItemIds
                            .Where(x => !storedTicketItemIds.Contains(x))
                            .ToList();

                        if (foreignTicketItemIds.Count > 0)
                        {
                            throw new InvalidOperationException($"Ticket item(s) " + $"{string.Join(", ", foreignTicketItemIds)} " + $"do not belong to ticket {target.Id}.");
                        }

                        // ---------------------------------------------
                        // DELETE Ticket Items
                        // ---------------------------------------------

                        var removedTicketItems = storedTicketItems
                            .Where(x => !keptTicketItemIds.Contains(x.Id))
                            .ToList();

                        if (removedTicketItems.Count > 0)
                        {
                            await _ticketItem.DeleteRange(removedTicketItems, token);
                        }

                        // ---------------------------------------------
                        // UPDATE existing Ticket Items
                        // ---------------------------------------------

                        foreach (var ticketItemRequest in ticketItems.Where(x => x.Id != 0))
                        {
                            var ticketItem = storedTicketItems.First(x => x.Id == ticketItemRequest.Id);

                            _mapper.Map(ticketItemRequest, ticketItem);

                            // -----------------------------------------
                            // Resolve PickingPlanItemId
                            //
                            // Case 1:
                            // pickingPlanItemId = -1
                            // => tìm trong itemIdMap
                            //
                            // Case 2:
                            // pickingPlanItemId = 1
                            // => DB ID hiện tại
                            // -----------------------------------------

                            if (ticketItemRequest.PickingPlanItemId.HasValue)
                            {
                                var key = ticketItemRequest.PickingPlanItemId.Value;

                                if (itemIdMap.TryGetValue(key, out var mappedItemId))
                                {
                                    ticketItem.PickingPlanItemId = (long)mappedItemId;
                                }
                                else if (key > 0)
                                {
                                    ticketItem.PickingPlanItemId = (long)key;
                                }
                                else
                                {
                                    throw new InvalidOperationException($"PickingPlanItemId '{key}' " + $"could not be resolved.");
                                }
                            }
                            else
                            {
                                ticketItem.PickingPlanItemId = null;
                            }

                            await _ticketItem.Update(ticketItem, token);
                        }

                        // ---------------------------------------------
                        // INSERT new Ticket Items
                        // ---------------------------------------------

                        foreach (var ticketItemRequest in ticketItems.Where(x => x.Id == 0))
                        {
                            ulong? realPlanItemId = null;

                            if (ticketItemRequest.PickingPlanItemId.HasValue)
                            {
                                var key = ticketItemRequest.PickingPlanItemId.Value;

                                if (itemIdMap.TryGetValue(key, out var mappedItemId))
                                {
                                    realPlanItemId = mappedItemId;
                                }
                                else if (key > 0)
                                {
                                    realPlanItemId = (ulong)key;
                                }
                                else
                                {
                                    throw new InvalidOperationException($"PickingPlanItemId '{key}' " + $"could not be resolved.");
                                }
                            }

                            var newTicketItem = ToTicketItemEntity(ticketItemRequest, target.Id, realPlanItemId);

                            await _ticketItem.Add(newTicketItem, token);
                        }
                    }

                    // =================================================
                    // INSERT NEW TICKETS
                    // =================================================

                    foreach (var ticketRequest in tickets.Where(x => x.Id == 0))
                    {
                        var newTicket = ToTicketEntity(ticketRequest, id);

                        await _ticket.Add(newTicket, token);

                        // ---------------------------------------------
                        // Ticket Items
                        // ---------------------------------------------

                        if (ticketRequest.PickingTicketItems is null) continue;


                        foreach (var ticketItemRequest in ticketRequest.PickingTicketItems)
                        {
                            ulong? realPlanItemId = null;

                            if (ticketItemRequest.PickingPlanItemId.HasValue)
                            {
                                var key = ticketItemRequest.PickingPlanItemId.Value;

                                if (itemIdMap.TryGetValue(key, out var mappedItemId))
                                {
                                    realPlanItemId = mappedItemId;
                                }
                                else if (key > 0)
                                {
                                    realPlanItemId = (ulong)key;
                                }
                                else
                                {
                                    throw new InvalidOperationException($"PickingPlanItemId '{key}' " + $"could not be resolved.");
                                }
                            }

                            var newTicketItem = ToTicketItemEntity(ticketItemRequest, newTicket.Id, realPlanItemId);

                            await _ticketItem.Add(newTicketItem, token);
                        }
                    }
                }

                // =====================================================
                // RESULT
                // =====================================================

                return Result<PickingPlanDto>.Success(_mapper.Map<PickingPlanDto>(entity));

            }, ct);
        }


        private Entities.PickingPlanItem ToPlanItemEntity(PickingPlanItemRequest request, ulong pickingPlanId)
        {
            return new Entities.PickingPlanItem
            {
                PickingPlanId = (long)pickingPlanId,
                ProductId = (long)(request.ProductId ?? 0),
                UnitId = (long)(request.UnitId ?? 0),

                RequiredQty = request.RequiredQty,
                AllocatedQty = request.AllocatedQty,
                PickedQty = request.PickedQty,
                RemainingQty = request.RemainingQty,

                Status = request.Status,
                Remark = request.Remark
            };
        }

        private Entities.PickingPlanItemSource ToPlanItemSourceEntity(PickingPlanItemSourceRequest request, ulong pickingPlanItemId)
        {
            return new Entities.PickingPlanItemSource
            {
                PickingPlanItemId = (long)pickingPlanItemId,
                SourceId = (long)(request.SourceId ?? 0),
                SourceDetailId = (long)(request.SourceDetailId ?? 0),

                RequiredQty = request.RequiredQty,
                PickedQty = request.PickedQty
            };
        }

        private Entities.PickingPlanSource ToSourceEntity(PickingPlanSourceRequest source, ulong pickingPlanId)
        {
            var entity = _mapper.Map<Entities.PickingPlanSource>(source);
            entity.PickingPlanId = (long)pickingPlanId;
            return entity;
        }

        private Entities.PickingTicket ToTicketEntity(PickingTicketRequest request, ulong pickingPlanId)
        {
            return new Entities.PickingTicket
            {
                PickingPlanId = (long)pickingPlanId,
                TicketNo = request.TicketNo,
                WarehouseId = (long)(request.WarehouseId ?? 0),
                Status = request.Status,
                AssignedTo = (long)(request.AssignedTo ?? 0),

                StartedAt = request.StartedAt,
                CompletedAt = request.CompletedAt,

                Remark = request.Remark
            };
        }

        private Entities.PickingTicketItem ToTicketItemEntity(PickingTicketItemRequest request, ulong pickingTicketId, ulong? pickingPlanItemId)
        {
            return new Entities.PickingTicketItem
            {
                PickingTicketId = (long)pickingTicketId,

                PickingPlanItemId = (long?)(pickingPlanItemId ?? 0),

                ProductId = (long)(request.ProductId ?? 0),
                LocationId = (long?)(request.LocationId ?? 0),
                LotId = (long?)(request.LotId ?? 0),
                SerialNo = request.SerialNo,

                RequestedQty = request.RequestedQty,
                PickedQty = request.PickedQty,

                Status = request.Status,
                Remark = request.Remark
            };
        }
    }

    public class PickingPlanItemService : IPickingPlanItemService
    {
        private readonly IRepository<Entities.PickingPlanItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PickingPlanItemService(
            IRepository<Entities.PickingPlanItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PickingPlanItemDto>> CreateAsync(PickingPlanItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PickingPlanItem>(request);
            await _repository.Add(entity, ct);
            return Result<PickingPlanItemDto>.Success(_mapper.Map<PickingPlanItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Picking plan item {id} was not found.");
        }

        public async Task<List<PickingPlanItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PickingPlanItemDto>>(await _repository.GetAll(ct));

        public async Task<PickingPlanItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PickingPlanItemDto>(entity);
        }

        public async Task<Result<PickingPlanItemDto>> UpdateAsync(ulong id, PickingPlanItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PickingPlanItemDto>.Failure($"Picking plan item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PickingPlanItemDto>.Success(_mapper.Map<PickingPlanItemDto>(entity));
        }
    }

    public class PickingPlanItemSourceService : IPickingPlanItemSourceService
    {
        private readonly IRepository<Entities.PickingPlanItemSource> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PickingPlanItemSourceService(
            IRepository<Entities.PickingPlanItemSource> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PickingPlanItemSourceDto>> CreateAsync(PickingPlanItemSourceRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PickingPlanItemSource>(request);
            await _repository.Add(entity, ct);
            return Result<PickingPlanItemSourceDto>.Success(_mapper.Map<PickingPlanItemSourceDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Picking plan item source {id} was not found.");
        }

        public async Task<List<PickingPlanItemSourceDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PickingPlanItemSourceDto>>(await _repository.GetAll(ct));

        public async Task<PickingPlanItemSourceDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PickingPlanItemSourceDto>(entity);
        }

        public async Task<Result<PickingPlanItemSourceDto>> UpdateAsync(ulong id, PickingPlanItemSourceRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PickingPlanItemSourceDto>.Failure($"Picking plan item source {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PickingPlanItemSourceDto>.Success(_mapper.Map<PickingPlanItemSourceDto>(entity));
        }
    }

    public class PickingPlanSourceService : IPickingPlanSourceService
    {
        private readonly IRepository<Entities.PickingPlanSource> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PickingPlanSourceService(
            IRepository<Entities.PickingPlanSource> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PickingPlanSourceDto>> CreateAsync(PickingPlanSourceRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PickingPlanSource>(request);
            await _repository.Add(entity, ct);
            return Result<PickingPlanSourceDto>.Success(_mapper.Map<PickingPlanSourceDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Picking plan source {id} was not found.");
        }

        public async Task<List<PickingPlanSourceDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PickingPlanSourceDto>>(await _repository.GetAll(ct));
        public async Task<PickingPlanSourceDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PickingPlanSourceDto>(entity);
        }

        public async Task<Result<PickingPlanSourceDto>> UpdateAsync(ulong id, PickingPlanSourceRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PickingPlanSourceDto>.Failure($"Picking plan source {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PickingPlanSourceDto>.Success(_mapper.Map<PickingPlanSourceDto>(entity));
        }
    }

    public class PickingTicketService : IPickingTicketService
    {
        private readonly IRepository<Entities.PickingTicket> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PickingTicketService(
            IRepository<Entities.PickingTicket> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PickingTicketDto>> CreateAsync(PickingTicketRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PickingTicket>(request);
            await _repository.Add(entity, ct);
            return Result<PickingTicketDto>.Success(_mapper.Map<PickingTicketDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Picking ticket {id} was not found.");
        }

        public async Task<List<PickingTicketDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PickingTicketDto>>(await _repository.GetAll(ct));
        public async Task<PickingTicketDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PickingTicketDto>(entity);
        }

        public async Task<Result<PickingTicketDto>> UpdateAsync(ulong id, PickingTicketRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PickingTicketDto>.Failure($"Picking ticket {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PickingTicketDto>.Success(_mapper.Map<PickingTicketDto>(entity));
        }
    }

    public class PickingTicketItemService : IPickingTicketItemService
    {
        private readonly IRepository<Entities.PickingTicketItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PickingTicketItemService(
            IRepository<Entities.PickingTicketItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PickingTicketItemDto>> CreateAsync(PickingTicketItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PickingTicketItem>(request);
            await _repository.Add(entity, ct);
            return Result<PickingTicketItemDto>.Success(_mapper.Map<PickingTicketItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Picking ticket item {id} was not found.");
        }

        public async Task<List<PickingTicketItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PickingTicketItemDto>>(await _repository.GetAll(ct));
        public async Task<PickingTicketItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PickingTicketItemDto>(entity);
        }

        public async Task<Result<PickingTicketItemDto>> UpdateAsync(ulong id, PickingTicketItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PickingTicketItemDto>.Failure($"Picking ticket item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PickingTicketItemDto>.Success(_mapper.Map<PickingTicketItemDto>(entity));
        }
    }
}
