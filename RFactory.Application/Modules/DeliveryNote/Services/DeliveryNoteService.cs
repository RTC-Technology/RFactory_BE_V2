using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Application.Modules.PickingPlan.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.DeliveryNote.Services
{
    public class DeliveryNoteService : IDeliveryNoteService
    {
        private readonly IRepository<Entities.DeliveryNote> _repository;
        private readonly IRepository<Entities.DeliveryNoteItem> _itemRepository;
        private readonly IRepository<Entities.DeliveryNoteSource> _sourceRepository;
        private readonly IRepository<Entities.DeliveryNoteSender> _senderRepository;
        private readonly IRepository<Entities.DeliveryNoteReceiver> _receiverRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryNoteService(
            IRepository<Entities.DeliveryNote> repository,
            IRepository<Entities.DeliveryNoteItem> itemRepository,
            IRepository<Entities.DeliveryNoteSource> sourceRepository,
            IRepository<Entities.DeliveryNoteSender> senderRepository,
            IRepository<Entities.DeliveryNoteReceiver> receiverRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _sourceRepository = sourceRepository;
            _senderRepository = senderRepository;
            _receiverRepository = receiverRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DeliveryNoteDto>> CreateAsync(DeliveryNoteRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(t => t.DeliveryNo == request.DeliveryNo, ct);
            if (existing is not null)
            {
                return Result<DeliveryNoteDto>.Failure($"Delivery Note '{request.DeliveryNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.DeliveryNote>(request);
            var items = request.DeliveryNoteItems ?? new List<DeliveryNoteItemRequest>();
            var sources = request.DeliveryNoteSources ?? new List<DeliveryNoteSourceRequest>();

            var sender = _mapper.Map<Entities.DeliveryNoteSender>(request.DeliveryNoteSender);
            var receiver = _mapper.Map<Entities.DeliveryNoteReceiver>(request.DeliveryNoteReceiver);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // Two saves rather than one: the lines need the id the database generates for
                // the receipt, which is only known once the receipt is in.

                await _repository.Add(entity, token);

                sender.DeliveryNoteId = entity.Id;
                await _senderRepository.Add(sender, token);

                receiver.DeliveryNoteId = entity.Id;
                await _receiverRepository.Add(receiver, token);

                await _itemRepository.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);
                await _sourceRepository.AddRange(sources.Select(source => ToSourceEntity(source, entity.Id)).ToList(), token);

                return Result<DeliveryNoteDto>.Success(_mapper.Map<DeliveryNoteDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Delivery note {id} was not found.");
            }

            //var deliveryNoteId = (ulong)id;
            var sources = await _sourceRepository.Where(p => p.DeliveryNoteId == id, ct);
            var items = await _itemRepository.Where(x => x.DeliveryNoteId == id, ct);

            var sender = await _senderRepository.Where(x => x.DeliveryNoteId == id, ct);
            var receiver = await _receiverRepository.Where(x => x.DeliveryNoteId == id, ct);

            

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {

                await _sourceRepository.DeleteRange(sources, token);
                await _itemRepository.DeleteRange(items, token);

                await _senderRepository.DeleteRange(sender, token);
                await _receiverRepository.DeleteRange(receiver, token);
                await _repository.Delete(entity, token);

                return Result.Success();
            }, ct);
        }

        public async Task<List<DeliveryNoteDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DeliveryNoteDto>>(await _repository.GetAll(ct));

        public async Task<DeliveryNoteDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DeliveryNoteDto>(entity);
        }

        public async Task<Result<DeliveryNoteDto>> UpdateAsync(ulong id, DeliveryNoteRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DeliveryNoteDto>.Failure($"Delivery Note {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(t => t.Id != id && t.DeliveryNo == request.DeliveryNo, ct);
            if (existing is not null)
            {
                return Result<DeliveryNoteDto>.Failure($"Delivery Note '{request.DeliveryNo}' already exists.");
            }

            var sender = await _senderRepository.GetById(request.DeliveryNoteSender?.Id ?? 0, ct);
            var receiver = await _receiverRepository.GetById(request.DeliveryNoteReceiver?.Id ?? 0, ct);

            //Update items
            var storedItems = await _itemRepository.Where(l => l.DeliveryNoteId == id, ct);

            var items = request.DeliveryNoteItems;
            var keptItemIds = (items ?? new List<DeliveryNoteItemRequest>())
                .Where(item => item.Id != 0)
                .Select(item => item.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItem = keptItemIds.Where(itemId => storedItems.All(s => s.Id != itemId)).ToList();
            if (foreignItem.Count > 0)
            {
                return Result<DeliveryNoteDto>.Failure($"Line(s) {string.Join(", ", foreignItem)} do not belong to Delivery Note {id}.");
            }

            //Update sources
            var storedSources = await _sourceRepository.Where(l => l.DeliveryNoteId == id, ct);

            var sources = request.DeliveryNoteSources;
            var keptSourceIds = (sources ?? new List<DeliveryNoteSourceRequest>())
                .Where(source => source.Id != 0)
                .Select(source => source.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignSource = keptSourceIds.Where(sourceId => storedSources.All(s => s.Id != sourceId)).ToList();
            if (foreignSource.Count > 0)
            {
                return Result<DeliveryNoteDto>.Failure($"Line(s) {string.Join(", ", foreignSource)} do not belong to Delivery Note {id}.");
            }


            _mapper.Map(request, entity);
            _mapper.Map(request.DeliveryNoteSender, sender);
            _mapper.Map(request.DeliveryNoteReceiver, receiver);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repository.Update(entity, token);

                if (sender is not null) await _senderRepository.Update(sender, token);
                if (receiver is not null) await _receiverRepository.Update(receiver, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _itemRepository.DeleteRange(storedItems.Where(s => !keptItemIds.Contains(s.Id)).ToList(), token);

                    foreach (var item in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == item.Id);
                        _mapper.Map(item, target);
                        await _itemRepository.Update(target, token);
                    }

                    await _itemRepository.AddRange(items.Where(l => l.Id == 0).Select(item => ToItemEntity(item, id)).ToList(), token);

                }

                if (sources is not null)
                {
                    await _sourceRepository.DeleteRange(storedSources.Where(s => !keptSourceIds.Contains(s.Id)).ToList(), token);

                    foreach (var source in sources.Where(l => l.Id != 0))
                    {
                        var target = storedSources.First(s => s.Id == source.Id);
                        _mapper.Map(source, target);
                        await _sourceRepository.Update(target, token);
                    }

                    await _sourceRepository.AddRange(sources.Where(l => l.Id == 0).Select(source => ToSourceEntity(source, id)).ToList(), token);

                }

                return Result<DeliveryNoteDto>.Success(_mapper.Map<DeliveryNoteDto>(entity));
            }, ct);
        }


        private Entities.DeliveryNoteItem ToItemEntity(DeliveryNoteItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.DeliveryNoteItem>(line);
            entity.DeliveryNoteId = id;
            return entity;
        }
        private Entities.DeliveryNoteSource ToSourceEntity(DeliveryNoteSourceRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.DeliveryNoteSource>(line);
            entity.DeliveryNoteId = id;
            return entity;
        }
    }

    public class DeliveryNoteItemService : IDeliveryNoteItemService
    {
        private readonly IRepository<Entities.DeliveryNoteItem> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryNoteItemService(
            IRepository<Entities.DeliveryNoteItem> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DeliveryNoteItemDto>> CreateAsync(DeliveryNoteItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.DeliveryNoteItem>(request);
            await _repository.Add(entity, ct);
            return Result<DeliveryNoteItemDto>.Success(_mapper.Map<DeliveryNoteItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Delivery note item {id} was not found.");
        }

        public async Task<List<DeliveryNoteItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DeliveryNoteItemDto>>(await _repository.GetAll(ct));
        public async Task<DeliveryNoteItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DeliveryNoteItemDto>(entity);
        }

        public async Task<Result<DeliveryNoteItemDto>> UpdateAsync(ulong id, DeliveryNoteItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DeliveryNoteItemDto>.Failure($"Delivery note item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DeliveryNoteItemDto>.Success(_mapper.Map<DeliveryNoteItemDto>(entity));
        }
    }
    public class DeliveryNoteSenderService : IDeliveryNoteSenderService
    {
        private readonly IRepository<Entities.DeliveryNoteSender> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryNoteSenderService(
            IRepository<Entities.DeliveryNoteSender> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DeliveryNoteSenderDto>> CreateAsync(DeliveryNoteSenderRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.DeliveryNoteSender>(request);
            await _repository.Add(entity, ct);
            return Result<DeliveryNoteSenderDto>.Success(_mapper.Map<DeliveryNoteSenderDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Delivery note sender {id} was not found.");
        }

        public async Task<List<DeliveryNoteSenderDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DeliveryNoteSenderDto>>(await _repository.GetAll(ct));
        public async Task<DeliveryNoteSenderDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DeliveryNoteSenderDto>(entity);
        }

        public async Task<Result<DeliveryNoteSenderDto>> UpdateAsync(ulong id, DeliveryNoteSenderRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DeliveryNoteSenderDto>.Failure($"Delivery note sender {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DeliveryNoteSenderDto>.Success(_mapper.Map<DeliveryNoteSenderDto>(entity));
        }
    }
    public class DeliveryNoteSourceService : IDeliveryNoteSourceService
    {
        private readonly IRepository<Entities.DeliveryNoteSource> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryNoteSourceService(
            IRepository<Entities.DeliveryNoteSource> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DeliveryNoteSourceDto>> CreateAsync(DeliveryNoteSourceRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.DeliveryNoteSource>(request);
            await _repository.Add(entity, ct);
            return Result<DeliveryNoteSourceDto>.Success(_mapper.Map<DeliveryNoteSourceDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Delivery note source {id} was not found.");
        }

        public async Task<List<DeliveryNoteSourceDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DeliveryNoteSourceDto>>(await _repository.GetAll(ct));
        public async Task<DeliveryNoteSourceDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DeliveryNoteSourceDto>(entity);
        }

        public async Task<Result<DeliveryNoteSourceDto>> UpdateAsync(ulong id, DeliveryNoteSourceRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DeliveryNoteSourceDto>.Failure($"Delivery note source {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DeliveryNoteSourceDto>.Success(_mapper.Map<DeliveryNoteSourceDto>(entity));
        }
    }
    public class DeliveryNoteReceiverService : IDeliveryNoteReceiverService
    {
        private readonly IRepository<Entities.DeliveryNoteReceiver> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryNoteReceiverService(
            IRepository<Entities.DeliveryNoteReceiver> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DeliveryNoteReceiverDto>> CreateAsync(DeliveryNoteReceiverRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.DeliveryNoteReceiver>(request);
            await _repository.Add(entity, ct);
            return Result<DeliveryNoteReceiverDto>.Success(_mapper.Map<DeliveryNoteReceiverDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Delivery note receiver {id} was not found.");
        }

        public async Task<List<DeliveryNoteReceiverDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DeliveryNoteReceiverDto>>(await _repository.GetAll(ct));
        public async Task<DeliveryNoteReceiverDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DeliveryNoteReceiverDto>(entity);
        }

        public async Task<Result<DeliveryNoteReceiverDto>> UpdateAsync(ulong id, DeliveryNoteReceiverRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DeliveryNoteReceiverDto>.Failure($"Delivery note receiver {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DeliveryNoteReceiverDto>.Success(_mapper.Map<DeliveryNoteReceiverDto>(entity));
        }
    }
}