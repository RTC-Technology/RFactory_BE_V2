using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.DeliveryNote.Services;
using RFactory.Application.Modules.Packing.DTOs;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Packing.Services
{
    public class PackingCheckService : IPackingCheckService
    {
        private readonly IRepository<Entities.PackingCheck> _repository;
        private readonly IRepository<Entities.PackingCheckItem> _checkItemRepository;
        private readonly IRepository<Entities.PackingPackage> _packageRepository;
        private readonly IRepository<Entities.PackingPackageItem> _packageItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackingCheckService(
            IRepository<Entities.PackingCheck> repository,
            IRepository<Entities.PackingCheckItem> checkItemRepository,
            IRepository<Entities.PackingPackage> packageRepository,
            IRepository<Entities.PackingPackageItem> packageItemRepository,

            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _checkItemRepository = checkItemRepository;
            _packageRepository = packageRepository;
            _packageItemRepository = packageItemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PackingCheckDto>> CreateAsync(PackingCheckRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(t => t.CheckNo == request.CheckNo, ct);
            if (existing is not null)
            {
                return Result<PackingCheckDto>.Failure($"Packing Check '{request.CheckNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.PackingCheck>(request);
            var items = request.PackingCheckItems ?? new List<PackingCheckItemRequest>();
            var packages = request.PackingPackages ?? new List<PackingPackageRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // Two saves rather than one: the lines need the id the database generates for
                // the receipt, which is only known once the receipt is in.

                await _repository.Add(entity, token);

                var itemIdMap = new Dictionary<long, ulong>();

                foreach (var item in items)
                {
                    var itemEntity = ToItemEntity(item, entity.Id);
                    await _checkItemRepository.Add(itemEntity, token);

                    // Sau khi Add, EF phải có entity.Id
                    if (item.UId.HasValue) itemIdMap[item.UId.Value] = itemEntity.Id;
                }

                foreach (var package in packages)
                {
                    if (package.PackingPackageItems is null) continue;

                    foreach (var packageItem in package.PackingPackageItems)
                    {
                        if (packageItem.PackingCheckItemId is not { } tempId) continue;

                        if (itemIdMap.TryGetValue(tempId, out var itemId))
                        {
                            packageItem.PackingCheckItemId = (long)itemId;
                        }
                    }
                }

                foreach (var package in packages)
                {
                    var packageEntity = ToPackageEntity(package, entity.Id);
                    await _packageRepository.Add(packageEntity, token);

                    var packageItems = package.PackingPackageItems ?? new List<PackingPackageItemRequest>();
                    var pItems = packageItems.Select(packageItem => ToPackageItemEntity(packageItem, packageEntity.Id)).ToList();
                    await _packageItemRepository.AddRange(pItems, token);
                }

                return Result<PackingCheckDto>.Success(_mapper.Map<PackingCheckDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Packing check {id} was not found.");
            }

            //var deliveryNoteId = (ulong)id;
            var checkItems = await _checkItemRepository.Where(p => p.PackingCheckId == id, ct);
            var packages = await _packageRepository.Where(x => x.PackingCheckId == id, ct);
            var packageItems = await _packageItemRepository.Where(x => packages.Select(p => p.Id).Contains(x.PackingPackageId), ct);


            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {

                await _packageItemRepository.DeleteRange(packageItems, token);
                await _packageRepository.DeleteRange(packages, token);
                await _checkItemRepository.DeleteRange(checkItems, token);
                await _repository.Delete(entity, token);

                return Result.Success();
            }, ct);
        }

        public async Task<List<PackingCheckDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PackingCheckDto>>(await _repository.GetAll(ct));
        public async Task<PackingCheckDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PackingCheckDto>(entity);
        }

        public async Task<Result<PackingCheckDto>> UpdateAsync(ulong id,PackingCheckRequest request,CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PackingCheckDto>.Failure($"Packing Check {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(
                t => t.Id != id && t.CheckNo == request.CheckNo,
                ct);

            if (existing is not null)
            {
                return Result<PackingCheckDto>.Failure(
                    $"Packing Check '{request.CheckNo}' already exists.");
            }

            var items = request.PackingCheckItems;
            var packages = request.PackingPackages;

            var storedItems = await _checkItemRepository.Where(
                l => l.PackingCheckId == id,
                ct);

            var storedPackages = await _packageRepository.Where(
                l => l.PackingCheckId == id,
                ct);

            var keptItemIds = (items ?? new List<PackingCheckItemRequest>())
                .Where(item => item.Id != 0)
                .Select(item => item.Id)
                .ToHashSet();

            var foreignItem = keptItemIds
                .Where(itemId => storedItems.All(s => s.Id != itemId))
                .ToList();

            if (foreignItem.Count > 0)
            {
                return Result<PackingCheckDto>.Failure(
                    $"Item(s) {string.Join(", ", foreignItem)} do not belong to Packing Check {id}.");
            }

            var keptPackageIds = (packages ?? new List<PackingPackageRequest>())
                .Where(package => package.Id != 0)
                .Select(package => package.Id)
                .ToHashSet();

            var foreignPackage = keptPackageIds
                .Where(packageId => storedPackages.All(s => s.Id != packageId))
                .ToList();

            if (foreignPackage.Count > 0)
            {
                return Result<PackingCheckDto>.Failure(
                    $"Package(s) {string.Join(", ", foreignPackage)} do not belong to Packing Check {id}.");
            }

            var storedPackageItems = await _packageItemRepository.Where(
                l => storedPackages.Select(p => p.Id).Contains(l.PackingPackageId),
                ct);

            var packageItemIds = (packages ?? new List<PackingPackageRequest>())
                .SelectMany(p => p.PackingPackageItems ?? new List<PackingPackageItemRequest>())
                .Where(item => item.Id != 0)
                .Select(item => item.Id)
                .ToHashSet();

            var foreignPackageItems = packageItemIds
                .Where(itemId => storedPackageItems.All(s => s.Id != itemId))
                .ToList();

            if (foreignPackageItems.Count > 0)
            {
                return Result<PackingCheckDto>.Failure(
                    $"Package item(s) {string.Join(", ", foreignPackageItems)} do not belong to Packing Check {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repository.Update(entity, token);

                if (items is not null)
                {
                    await _checkItemRepository.DeleteRange(
                        storedItems
                            .Where(s => !keptItemIds.Contains(s.Id))
                            .ToList(),
                        token);

                    var itemIdMap = new Dictionary<long, ulong>();

                    foreach (var item in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == item.Id);

                        _mapper.Map(item, target);

                        await _checkItemRepository.Update(target, token);

                        if (item.UId.HasValue)
                        {
                            itemIdMap[item.UId.Value] = target.Id;
                        }
                    }

                    foreach (var item in items.Where(l => l.Id == 0))
                    {
                        var itemEntity = ToItemEntity(item, id);

                        await _checkItemRepository.Add(itemEntity, token);

                        if (item.UId.HasValue)
                        {
                            itemIdMap[item.UId.Value] = itemEntity.Id;
                        }
                    }

                    foreach (var package in packages ?? new List<PackingPackageRequest>())
                    {
                        foreach (var packageItem in package.PackingPackageItems ??
                                 new List<PackingPackageItemRequest>())
                        {
                            if (packageItem.PackingCheckItemId is not { } tempId)
                            {
                                continue;
                            }

                            if (tempId < 0 &&
                                itemIdMap.TryGetValue(tempId, out var itemId))
                            {
                                packageItem.PackingCheckItemId = (long)itemId;
                            }
                        }
                    }
                }

                if (packages is not null)
                {
                    var deletedPackages = storedPackages
                        .Where(s => !keptPackageIds.Contains(s.Id))
                        .ToList();

                    var deletedPackageIds = deletedPackages
                        .Select(p => p.Id)
                        .ToHashSet();

                    var deletedPackageItems = storedPackageItems
                        .Where(s => deletedPackageIds.Contains(s.PackingPackageId))
                        .ToList();

                    if (deletedPackageItems.Count > 0)
                    {
                        await _packageItemRepository.DeleteRange(
                            deletedPackageItems,
                            token);
                    }

                    await _packageRepository.DeleteRange(
                        deletedPackages,
                        token);

                    foreach (var package in packages.Where(l => l.Id != 0))
                    {
                        var target = storedPackages.First(s => s.Id == package.Id);

                        _mapper.Map(package, target);

                        await _packageRepository.Update(target, token);

                        await UpdatePackageItems(
                            package,
                            target.Id,
                            storedPackageItems,
                            token);
                    }

                    foreach (var package in packages.Where(l => l.Id == 0))
                    {
                        var packageEntity = ToPackageEntity(package, id);

                        await _packageRepository.Add(packageEntity, token);

                        var packageItems = package.PackingPackageItems ??
                                           new List<PackingPackageItemRequest>();

                        if (packageItems.Count > 0)
                        {
                            var packageItemEntities = packageItems
                                .Select(packageItem =>
                                    ToPackageItemEntity(packageItem, packageEntity.Id))
                                .ToList();

                            await _packageItemRepository.AddRange(
                                packageItemEntities,
                                token);
                        }
                    }
                }

                return Result<PackingCheckDto>.Success(
                    _mapper.Map<PackingCheckDto>(entity));
            }, ct);
        }

        private Entities.PackingCheckItem ToItemEntity(PackingCheckItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.PackingCheckItem>(line);
            entity.PackingCheckId = id;
            return entity;
        }
        private Entities.PackingPackage ToPackageEntity(PackingPackageRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.PackingPackage>(line);
            entity.PackingCheckId = id;
            return entity;
        }
        private Entities.PackingPackageItem ToPackageItemEntity(PackingPackageItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.PackingPackageItem>(line);
            entity.PackingPackageId = id;
            entity.PackingCheckItemId = (ulong)(line.PackingCheckItemId ?? 0);
            return entity;
        }

        private async Task UpdatePackageItems(PackingPackageRequest package, ulong packageId, IEnumerable<Entities.PackingPackageItem> storedPackageItems,
            CancellationToken token)
        {
            var packageItems = package.PackingPackageItems;

            if (packageItems is null) return;

            var storedItems = storedPackageItems
                .Where(x => x.PackingPackageId == packageId)
                .ToList();

            var keptIds = packageItems
                .Where(x => x.Id != 0)
                .Select(x => x.Id)
                .ToHashSet();

            await _packageItemRepository.DeleteRange(storedItems.Where(x => !keptIds.Contains(x.Id)).ToList(), token);

            foreach (var packageItem in packageItems.Where(x => x.Id != 0))
            {
                var target = storedItems.First(x => x.Id == packageItem.Id);

                _mapper.Map(packageItem, target);

                await _packageItemRepository.Update(target, token);
            }

            var newItems = packageItems
                .Where(x => x.Id == 0)
                .Select(x => ToPackageItemEntity(x, packageId))
                .ToList();

            if (newItems.Count > 0) await _packageItemRepository.AddRange(newItems, token);

        }
    }

    public class PackingCheckItemService : IPackingCheckItemService
    {
        private readonly IRepository<Entities.PackingCheckItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackingCheckItemService(
            IRepository<Entities.PackingCheckItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PackingCheckItemDto>> CreateAsync(PackingCheckItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PackingCheckItem>(request);
            await _repository.Add(entity, ct);
            return Result<PackingCheckItemDto>.Success(_mapper.Map<PackingCheckItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Packing check item {id} was not found.");
        }

        public async Task<List<PackingCheckItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PackingCheckItemDto>>(await _repository.GetAll(ct));
        public async Task<PackingCheckItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PackingCheckItemDto>(entity);
        }

        public async Task<Result<PackingCheckItemDto>> UpdateAsync(ulong id, PackingCheckItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PackingCheckItemDto>.Failure($"Packing check item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PackingCheckItemDto>.Success(_mapper.Map<PackingCheckItemDto>(entity));
        }
    }

    public class PackingPackageService : IPackingPackageService
    {
        private readonly IRepository<Entities.PackingPackage> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackingPackageService(
            IRepository<Entities.PackingPackage> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PackingPackageDto>> CreateAsync(PackingPackageRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PackingPackage>(request);
            await _repository.Add(entity, ct);
            return Result<PackingPackageDto>.Success(_mapper.Map<PackingPackageDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Packing package {id} was not found.");
        }

        public async Task<List<PackingPackageDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PackingPackageDto>>(await _repository.GetAll(ct));
        public async Task<PackingPackageDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PackingPackageDto>(entity);
        }

        public async Task<Result<PackingPackageDto>> UpdateAsync(ulong id, PackingPackageRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PackingPackageDto>.Failure($"Packing package {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PackingPackageDto>.Success(_mapper.Map<PackingPackageDto>(entity));
        }
    }

    public class PackingPackageItemService : IPackingPackageItemService
    {
        private readonly IRepository<Entities.PackingPackageItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackingPackageItemService(
            IRepository<Entities.PackingPackageItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PackingPackageItemDto>> CreateAsync(PackingPackageItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PackingPackageItem>(request);
            await _repository.Add(entity, ct);
            return Result<PackingPackageItemDto>.Success(_mapper.Map<PackingPackageItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Packing package item {id} was not found.");
        }

        public async Task<List<PackingPackageItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PackingPackageItemDto>>(await _repository.GetAll(ct));
        public async Task<PackingPackageItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PackingPackageItemDto>(entity);
        }

        public async Task<Result<PackingPackageItemDto>> UpdateAsync(ulong id, PackingPackageItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PackingPackageItemDto>.Failure($"Packing package item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PackingPackageItemDto>.Success(_mapper.Map<PackingPackageItemDto>(entity));
        }
    }

    public class PackingScanLogService : IPackingScanLogService
    {
        private readonly IRepository<Entities.PackingScanLog> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackingScanLogService(
            IRepository<Entities.PackingScanLog> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PackingScanLogDto>> CreateAsync(PackingScanLogRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PackingScanLog>(request);
            await _repository.Add(entity, ct);
            return Result<PackingScanLogDto>.Success(_mapper.Map<PackingScanLogDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Packing scan log {id} was not found.");
        }

        public async Task<List<PackingScanLogDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<PackingScanLogDto>>(await _repository.GetAll(ct));
        public async Task<PackingScanLogDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PackingScanLogDto>(entity);
        }

        public async Task<Result<PackingScanLogDto>> UpdateAsync(ulong id, PackingScanLogRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<PackingScanLogDto>.Failure($"Packing scan log {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<PackingScanLogDto>.Success(_mapper.Map<PackingScanLogDto>(entity));
        }
    }
}
