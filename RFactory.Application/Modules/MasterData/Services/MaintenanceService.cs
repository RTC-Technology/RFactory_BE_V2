using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.MasterData.Services
{
    public class MaintenanceTypeService : IMaintenanceTypeService
    {
        private readonly IRepository<Entities.MaintenanceType> _repository;
        private readonly IMapper _mapper;

        public MaintenanceTypeService(
            IRepository<Entities.MaintenanceType> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MaintenanceTypeDto>> CreateAsync(MaintenanceTypeRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.TypeCode == request.TypeCode, ct);
            if (existing is not null)
            {
                return Result<MaintenanceTypeDto>.Failure($"Maintenance type code '{request.TypeCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.MaintenanceType>(request);
            await _repository.Add(entity, ct);
            return Result<MaintenanceTypeDto>.Success(_mapper.Map<MaintenanceTypeDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Maintenance type {id} was not found.");
        }

        public async Task<List<MaintenanceTypeDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<MaintenanceTypeDto>>(await _repository.GetAll(ct));
        public async Task<MaintenanceTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenanceTypeDto>(entity);
        }

        public async Task<Result<MaintenanceTypeDto>> UpdateAsync(ulong id, MaintenanceTypeRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<MaintenanceTypeDto>.Failure($"Maintenance type {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.TypeCode == request.TypeCode, ct);
            if (existing is not null)
            {
                return Result<MaintenanceTypeDto>.Failure($"Maintenance type code '{request.TypeCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<MaintenanceTypeDto>.Success(_mapper.Map<MaintenanceTypeDto>(entity));
        }
    }
    public class MaintenanceChecklistService : IMaintenanceChecklistService
    {
        private readonly IRepository<Entities.MaintenanceChecklist> _repo;
        private readonly IRepository<Entities.MaintenanceChecklistItem> _itemRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MaintenanceChecklistService(
            IRepository<Entities.MaintenanceChecklist> repo,
            IRepository<Entities.MaintenanceChecklistItem> itemRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _itemRepo = itemRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<MaintenanceChecklistDto>> CreateAsync(MaintenanceChecklistRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.ChecklistCode == request.ChecklistCode, ct);
            if (existing is not null)
            {
                return Result<MaintenanceChecklistDto>.Failure($"Maintenance checklist '{request.ChecklistCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.MaintenanceChecklist>(request);
            var items = request.MaintenanceChecklistItems ?? new List<MaintenanceChecklistItemRequest>();
            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _itemRepo.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);

                return Result<MaintenanceChecklistDto>.Success(_mapper.Map<MaintenanceChecklistDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Maintenance checklist {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _itemRepo.Where(p => p.MaintenanceChecklistId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _itemRepo.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<MaintenanceChecklistDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<MaintenanceChecklistDto>>(await _repo.GetAll(ct));

        public async Task<MaintenanceChecklistDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenanceChecklistDto>(entity);
        }

        public async Task<Result<MaintenanceChecklistDto>> UpdateAsync(ulong id, MaintenanceChecklistRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<MaintenanceChecklistDto>.Failure($"Maintenance checklist {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.ChecklistCode == request.ChecklistCode, ct);
            if (existing is not null)
            {
                return Result<MaintenanceChecklistDto>.Failure($"Maintenance checklist '{request.ChecklistCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storeds = await _itemRepo.Where(l => l.MaintenanceChecklistId == id, ct);
            var items = request.MaintenanceChecklistItems;
            var keptIds = (items ?? new List<MaintenanceChecklistItemRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreigns = keptIds.Where(lineId => storeds.All(s => s.Id != lineId)).ToList();
            if (foreigns.Count > 0)
            {
                return Result<MaintenanceChecklistDto>.Failure($"Item(s) {string.Join(", ", foreigns)} do not belong to Maintenance checklist {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _itemRepo.DeleteRange(storeds.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in items.Where(l => l.Id != 0))
                    {
                        var target = storeds.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _itemRepo.Update(target, token);
                    }

                    await _itemRepo.AddRange(items.Where(l => l.Id == 0).Select(line => ToItemEntity(line, id)).ToList(), token);
                }

                return Result<MaintenanceChecklistDto>.Success(_mapper.Map<MaintenanceChecklistDto>(entity));
            }, ct);
        }

        private Entities.MaintenanceChecklistItem ToItemEntity(MaintenanceChecklistItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.MaintenanceChecklistItem>(line);
            entity.MaintenanceChecklistId = id;
            return entity;
        }
    }
    public class MaintenanceChecklistItemService : IMaintenanceChecklistItemService
    {
        private readonly IRepository<Entities.MaintenanceChecklistItem> _repository;
        private readonly IMapper _mapper;

        public MaintenanceChecklistItemService(
            IRepository<Entities.MaintenanceChecklistItem> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MaintenanceChecklistItemDto>> CreateAsync(MaintenanceChecklistItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.MaintenanceChecklistItem>(request);
            await _repository.Add(entity, ct);
            return Result<MaintenanceChecklistItemDto>.Success(_mapper.Map<MaintenanceChecklistItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Maintenance checklist item {id} was not found.");
        }

        public async Task<List<MaintenanceChecklistItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<MaintenanceChecklistItemDto>>(await _repository.GetAll(ct));
        public async Task<MaintenanceChecklistItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenanceChecklistItemDto>(entity);
        }

        public async Task<Result<MaintenanceChecklistItemDto>> UpdateAsync(ulong id, MaintenanceChecklistItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<MaintenanceChecklistItemDto>.Failure($"Maintenance checklist item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<MaintenanceChecklistItemDto>.Success(_mapper.Map<MaintenanceChecklistItemDto>(entity));
        }
    }
    public class MaintenancePlanService : IMaintenancePlanService
    {
        private readonly IRepository<Entities.MaintenancePlan> _repository;
        private readonly IMapper _mapper;

        public MaintenancePlanService(
            IRepository<Entities.MaintenancePlan> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MaintenancePlanDto>> CreateAsync(MaintenancePlanRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.PlanCode == request.PlanCode, ct);
            if (existing is not null)
            {
                return Result<MaintenancePlanDto>.Failure($"Maintenance plan code '{request.PlanCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.MaintenancePlan>(request);
            await _repository.Add(entity, ct);
            return Result<MaintenancePlanDto>.Success(_mapper.Map<MaintenancePlanDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Maintenance plan {id} was not found.");
        }

        public async Task<List<MaintenancePlanDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<MaintenancePlanDto>>(await _repository.GetAll(ct));
        public async Task<MaintenancePlanDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenancePlanDto>(entity);
        }

        public async Task<Result<MaintenancePlanDto>> UpdateAsync(ulong id, MaintenancePlanRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<MaintenancePlanDto>.Failure($"Maintenance plan {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.PlanCode == request.PlanCode, ct);
            if (existing is not null)
            {
                return Result<MaintenancePlanDto>.Failure($"Maintenance plan code '{request.PlanCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<MaintenancePlanDto>.Success(_mapper.Map<MaintenancePlanDto>(entity));
        }
    }

    #region Maintenance Order Services
    public class MaintenanceOrderService : IMaintenanceOrderService
    {
        private readonly IRepository<Entities.MaintenanceOrder> _repo;
        private readonly IRepository<Entities.MaintenanceOrderChecklist> _checklistRepo;
        private readonly IRepository<Entities.MaintenanceOrderChecklistItem> _checklistItemRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MaintenanceOrderService(
            IRepository<Entities.MaintenanceOrder> repo,
            IRepository<Entities.MaintenanceOrderChecklist> checklistRepo,
            IRepository<Entities.MaintenanceOrderChecklistItem> checklistItemRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _checklistRepo = checklistRepo;
            _checklistItemRepo = checklistItemRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<MaintenanceOrderDto>> CreateAsync(MaintenanceOrderRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.OrderNo == request.OrderNo, ct);
            if (existing is not null)
            {
                return Result<MaintenanceOrderDto>.Failure($"Maintenance order '{request.OrderNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.MaintenanceOrder>(request);
            var checklists = request.MaintenanceOrderChecklists ?? new List<MaintenanceOrderChecklistRequest>();
            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                foreach (var checklist in checklists)
                {
                    var checklistEntity = ToChecklistEntity(checklist, entity.Id);
                    await _checklistRepo.Add(checklistEntity, token);

                    var checklistItems = checklist.MaintenanceOrderChecklistItems ?? new List<MaintenanceOrderChecklistItemRequest>();
                    await _checklistItemRepo.AddRange(checklistItems.Select(item => ToChecklistItemEntity(item, checklistEntity.Id)).ToList(), token);
                }

                return Result<MaintenanceOrderDto>.Success(_mapper.Map<MaintenanceOrderDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Maintenance order {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _checklistRepo.Where(p => p.MaintenanceOrderId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _checklistRepo.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<MaintenanceOrderDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<MaintenanceOrderDto>>(await _repo.GetAll(ct));

        public async Task<MaintenanceOrderDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenanceOrderDto>(entity);
        }

        public async Task<Result<MaintenanceOrderDto>> UpdateAsync(ulong id, MaintenanceOrderRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);

            if (entity is null)
            {
                return Result<MaintenanceOrderDto>.Failure($"Maintenance order {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.OrderNo == request.OrderNo, ct);

            if (existing is not null)
            {
                return Result<MaintenanceOrderDto>.Failure($"Maintenance order '{request.OrderNo}' already exists.");
            }

            var storedChecklists = await _checklistRepo.Where(x => x.MaintenanceOrderId == id, ct);

            var checklists = request.MaintenanceOrderChecklists;

            // ============================================================
            // 1. Validate Checklist IDs
            // ============================================================

            var keptChecklistIds = (checklists ?? new List<MaintenanceOrderChecklistRequest>())
                .Where(x => x.Id != 0)
                .Select(x => x.Id)
                .ToHashSet();

            var foreignChecklistIds = keptChecklistIds
                .Where(checklistId => storedChecklists.All(x => x.Id != checklistId))
                .ToList();

            if (foreignChecklistIds.Count > 0)
            {
                return Result<MaintenanceOrderDto>.Failure($"Checklist(s) {string.Join(", ", foreignChecklistIds)} " + $"do not belong to Maintenance order {id}.");
            }

            // ============================================================
            // 2. Load all existing Checklist Items
            // ============================================================

            var storedChecklistIds = storedChecklists.Select(x => x.Id).ToHashSet();

            var storedChecklistItems = storedChecklistIds.Count > 0
                ? await _checklistItemRepo.Where(x => storedChecklistIds.Contains(x.MaintenanceOrderChecklistId ?? 0), ct)
                : new List<Entities.MaintenanceOrderChecklistItem>();

            // ============================================================
            // 3. Validate Checklist Item IDs
            // ============================================================

            var requestChecklistItems = (checklists ?? new List<MaintenanceOrderChecklistRequest>())
                .SelectMany(checklist => checklist.MaintenanceOrderChecklistItems ?? new List<MaintenanceOrderChecklistItemRequest>())
                .ToList();

            var keptChecklistItemIds = requestChecklistItems
                .Where(x => x.Id != 0)
                .Select(x => x.Id)
                .ToHashSet();

            var foreignChecklistItemIds = keptChecklistItemIds
                .Where(itemId => storedChecklistItems.All(x => x.Id != itemId))
                .ToList();

            if (foreignChecklistItemIds.Count > 0)
            {
                return Result<MaintenanceOrderDto>.Failure($"Checklist item(s) {string.Join(", ", foreignChecklistItemIds)} " + $"do not belong to Maintenance order {id}.");
            }

            // ============================================================
            // 4. Validate Item belongs to the Checklist in request
            // ============================================================

            foreach (var checklist in checklists ?? new List<MaintenanceOrderChecklistRequest>())
            {
                if (checklist.Id == 0) continue;

                var requestItemIds = (checklist.MaintenanceOrderChecklistItems ?? new List<MaintenanceOrderChecklistItemRequest>())
                    .Where(x => x.Id != 0)
                    .Select(x => x.Id)
                    .ToHashSet();

                var storedItemIds = storedChecklistItems
                    .Where(x => x.MaintenanceOrderChecklistId == checklist.Id)
                    .Select(x => x.Id)
                    .ToHashSet();

                var foreignItemIds = requestItemIds
                    .Where(itemId => !storedItemIds.Contains(itemId))
                    .ToList();

                if (foreignItemIds.Count > 0)
                {
                    return Result<MaintenanceOrderDto>.Failure($"Checklist item(s) {string.Join(", ", foreignItemIds)} " + $"do not belong to Checklist {checklist.Id}.");
                }
            }

            // ============================================================
            // 5. Map Header
            // ============================================================

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // ========================================================
                // 6. Update MaintenanceOrder
                // ========================================================

                await _repo.Update(entity, token);

                // ========================================================
                // 7. Update Checklist
                // ========================================================

                if (checklists is not null)
                {
                    // --------------------------------------------
                    // 7.1 Delete Checklist không còn trong request
                    // --------------------------------------------

                    var deletedChecklists = storedChecklists.Where(x => !keptChecklistIds.Contains(x.Id)).ToList();

                    if (deletedChecklists.Count > 0)
                    {
                        // Xóa item trước
                        var deletedChecklistIds = deletedChecklists.Select(x => x.Id).ToHashSet();

                        var deletedItems = storedChecklistItems.Where(x => deletedChecklistIds.Contains(x.MaintenanceOrderChecklistId ?? 0)).ToList();

                        if (deletedItems.Count > 0) await _checklistItemRepo.DeleteRange(deletedItems, token);


                        // Sau đó xóa checklist
                        await _checklistRepo.DeleteRange(deletedChecklists, token);
                    }

                    // --------------------------------------------
                    // 7.2 Update / Add Checklist
                    // --------------------------------------------

                    foreach (var checklistRequest in checklists)
                    {
                        if (checklistRequest.Id != 0)
                        {
                            // ================================
                            // Existing Checklist
                            // ================================

                            var checklistEntity = storedChecklists.First(x => x.Id == checklistRequest.Id);

                            _mapper.Map(checklistRequest, checklistEntity);

                            await _checklistRepo.Update(checklistEntity, token);

                            // --------------------------------
                            // Update Items
                            // --------------------------------

                            var requestItems = checklistRequest.MaintenanceOrderChecklistItems ?? new List<MaintenanceOrderChecklistItemRequest>();

                            var storedItems = storedChecklistItems
                                .Where(x => x.MaintenanceOrderChecklistId == checklistEntity.Id)
                                .ToList();

                            var keptItemIds = requestItems
                                .Where(x => x.Id != 0)
                                .Select(x => x.Id)
                                .ToHashSet();

                            // Delete items
                            var deletedItems = storedItems
                                .Where(x => !keptItemIds.Contains(x.Id))
                                .ToList();

                            if (deletedItems.Count > 0) await _checklistItemRepo.DeleteRange(deletedItems, token);


                            // Update existing items
                            foreach (var itemRequest in requestItems.Where(x => x.Id != 0))
                            {
                                var itemEntity = storedItems.First(x => x.Id == itemRequest.Id);

                                _mapper.Map(itemRequest, itemEntity);

                                await _checklistItemRepo.Update(itemEntity, token);
                            }

                            // Add new items
                            var newItems = requestItems
                                .Where(x => x.Id == 0)
                                .Select(x => ToChecklistItemEntity(x, checklistEntity.Id)).ToList();

                            if (newItems.Count > 0) await _checklistItemRepo.AddRange(newItems, token);

                        }
                        else
                        {
                            // ================================
                            // New Checklist
                            // ================================

                            var checklistEntity = ToChecklistEntity(checklistRequest, id);

                            await _checklistRepo.Add(checklistEntity, token);

                            // New Checklist đã có Id
                            // => insert Items
                            var requestItems = checklistRequest.MaintenanceOrderChecklistItems ?? new List<MaintenanceOrderChecklistItemRequest>();

                            var newItems = requestItems.Select(x => ToChecklistItemEntity(x, checklistEntity.Id)).ToList();

                            if (newItems.Count > 0) await _checklistItemRepo.AddRange(newItems, token);

                        }
                    }
                }

                return Result<MaintenanceOrderDto>.Success(_mapper.Map<MaintenanceOrderDto>(entity));

            }, ct);
        }

        private Entities.MaintenanceOrderChecklist ToChecklistEntity(MaintenanceOrderChecklistRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.MaintenanceOrderChecklist>(line);
            entity.MaintenanceOrderId = id;
            return entity;
        }

        private Entities.MaintenanceOrderChecklistItem ToChecklistItemEntity(MaintenanceOrderChecklistItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.MaintenanceOrderChecklistItem>(line);
            entity.MaintenanceOrderChecklistId = id;
            return entity;
        }
    }
    public class MaintenanceOrderChecklistService : IMaintenanceOrderChecklistService
    {
        private readonly IRepository<Entities.MaintenanceOrderChecklist> _repository;
        private readonly IMapper _mapper;

        public MaintenanceOrderChecklistService(
            IRepository<Entities.MaintenanceOrderChecklist> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MaintenanceOrderChecklistDto>> CreateAsync(MaintenanceOrderChecklistRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.MaintenanceOrderChecklist>(request);
            await _repository.Add(entity, ct);
            return Result<MaintenanceOrderChecklistDto>.Success(_mapper.Map<MaintenanceOrderChecklistDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Maintenance order checklist {id} was not found.");
        }

        public async Task<List<MaintenanceOrderChecklistDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<MaintenanceOrderChecklistDto>>(await _repository.GetAll(ct));
        public async Task<MaintenanceOrderChecklistDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenanceOrderChecklistDto>(entity);
        }

        public async Task<Result<MaintenanceOrderChecklistDto>> UpdateAsync(ulong id, MaintenanceOrderChecklistRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<MaintenanceOrderChecklistDto>.Failure($"Maintenance order checklist {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<MaintenanceOrderChecklistDto>.Success(_mapper.Map<MaintenanceOrderChecklistDto>(entity));
        }
    }
    public class MaintenanceOrderChecklistItemService : IMaintenanceOrderChecklistItemService
    {
        private readonly IRepository<Entities.MaintenanceOrderChecklistItem> _repository;
        private readonly IMapper _mapper;

        public MaintenanceOrderChecklistItemService(
            IRepository<Entities.MaintenanceOrderChecklistItem> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MaintenanceOrderChecklistItemDto>> CreateAsync(MaintenanceOrderChecklistItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.MaintenanceOrderChecklistItem>(request);
            await _repository.Add(entity, ct);
            return Result<MaintenanceOrderChecklistItemDto>.Success(_mapper.Map<MaintenanceOrderChecklistItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Maintenance order checklist item {id} was not found.");
        }

        public async Task<List<MaintenanceOrderChecklistItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<MaintenanceOrderChecklistItemDto>>(await _repository.GetAll(ct));
        public async Task<MaintenanceOrderChecklistItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<MaintenanceOrderChecklistItemDto>(entity);
        }

        public async Task<Result<MaintenanceOrderChecklistItemDto>> UpdateAsync(ulong id, MaintenanceOrderChecklistItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<MaintenanceOrderChecklistItemDto>.Failure($"Maintenance order checklist item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<MaintenanceOrderChecklistItemDto>.Success(_mapper.Map<MaintenanceOrderChecklistItemDto>(entity));
        }
    }

    #endregion
}
