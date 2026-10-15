using AutoMapper;
using RFactory.Application.Modules.Production.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Production.Services
{
    public class SalesOrderService:ISalesOrderService
    {
        private readonly IRepository<Entities.SalesOrder> _repo;
        private readonly IRepository<Entities.SalesOrderItem> _itemRepo;
        private readonly IRepository<Entities.SalesOrderPaymentSchedule> _paymentScheduleRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalesOrderService(
            IRepository<Entities.SalesOrder> repo,
            IRepository<Entities.SalesOrderItem> itemRepo,
            IRepository<Entities.SalesOrderPaymentSchedule> paymentScheduleRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _itemRepo = itemRepo;
            _paymentScheduleRepo = paymentScheduleRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<SalesOrderDto>> CreateAsync(SalesOrderRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.OrderNo == request.OrderNo, ct);
            if (existing is not null)
            {
                return Result<SalesOrderDto>.Failure($"Sales order '{request.OrderNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.SalesOrder>(request);
            var items = request.SalesOrderItems ?? new List<SalesOrderItemRequest>();
            var paymentSchedules = request.SalesOrderPaymentSchedules ?? new List<SalesOrderPaymentScheduleRequest  >();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _itemRepo.AddRange(items.Select(item => ToItemEntity(item, entity.Id)).ToList(), token);

                await _paymentScheduleRepo.AddRange(paymentSchedules.Select(schedule => ToPaymentScheduleEntity(schedule, entity.Id)).ToList(), token);

                return Result<SalesOrderDto>.Success(_mapper.Map<SalesOrderDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Sales order {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _itemRepo.Where(p => p.SalesOrderId == id, ct);
            var paymentSchedules = await _paymentScheduleRepo.Where(p => p.SalesOrderId == id, ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _itemRepo.DeleteRange(items, token);
                await _paymentScheduleRepo.DeleteRange(paymentSchedules, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<SalesOrderDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<SalesOrderDto>>(await _repo.GetAll(ct));

        public async Task<SalesOrderDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SalesOrderDto>(entity);
        }

        public async Task<Result<SalesOrderDto>> UpdateAsync(ulong id, SalesOrderRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<SalesOrderDto>.Failure($"Sales order {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.OrderNo == request.OrderNo, ct);
            if (existing is not null)
            {
                return Result<SalesOrderDto>.Failure($"Sales order '{request.OrderNo}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _itemRepo.Where(l => l.SalesOrderId == id, ct);
            var items = request.SalesOrderItems;
            var keptItemIds = (items ?? new List<SalesOrderItemRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignItems = keptItemIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreignItems.Count > 0)
            {
                return Result<SalesOrderDto>.Failure($"Item(s) {string.Join(", ", foreignItems)} do not belong to Sales order {id}.");
            }

            var storedSchedules = await _paymentScheduleRepo.Where(l => l.SalesOrderId == id, ct);
            var paymentSchedules = request.SalesOrderPaymentSchedules;
            var keptScheduleIds = (paymentSchedules ?? new List<SalesOrderPaymentScheduleRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreignSchedules = keptScheduleIds.Where(lineId => storedSchedules.All(s => s.Id != lineId)).ToList();
            if (foreignSchedules.Count > 0)
            {
                return Result<SalesOrderDto>.Failure($"Payment schedule(s) {string.Join(", ", foreignSchedules)} do not belong to Sales order {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (items is not null)
                {
                    await _itemRepo.DeleteRange(storedItems.Where(s => !keptItemIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in items.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _itemRepo.Update(target, token);
                    }

                    await _itemRepo.AddRange(items.Where(l => l.Id == 0).Select(line => ToItemEntity(line, id)).ToList(), token);
                }

                if (paymentSchedules is not null)
                {
                    await _paymentScheduleRepo.DeleteRange(storedSchedules.Where(s => !keptScheduleIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in paymentSchedules.Where(l => l.Id != 0))
                    {
                        var target = storedSchedules.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _paymentScheduleRepo.Update(target, token);
                    }

                    await _paymentScheduleRepo.AddRange(paymentSchedules.Where(l => l.Id == 0).Select(line => ToPaymentScheduleEntity(line, id)).ToList(), token);
                }

                return Result<SalesOrderDto>.Success(_mapper.Map<SalesOrderDto>(entity));
            }, ct);
        }

        private Entities.SalesOrderItem ToItemEntity(SalesOrderItemRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.SalesOrderItem>(line);
            entity.SalesOrderId = id;
            return entity;
        }
        private Entities.SalesOrderPaymentSchedule ToPaymentScheduleEntity(SalesOrderPaymentScheduleRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.SalesOrderPaymentSchedule>(line);
            entity.SalesOrderId = id;
            return entity;
        }
    }

    public class SalesOrderItemService : ISalesOrderItemService
    {
        private readonly IRepository<Entities.SalesOrderItem> _repository;
        private readonly IMapper _mapper;

        public SalesOrderItemService(
            IRepository<Entities.SalesOrderItem> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SalesOrderItemDto>> CreateAsync(SalesOrderItemRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.SalesOrderItem>(request);
            await _repository.Add(entity, ct);
            return Result<SalesOrderItemDto>.Success(_mapper.Map<SalesOrderItemDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Sales order item {id} was not found.");
        }

        public async Task<List<SalesOrderItemDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<SalesOrderItemDto>>(await _repository.GetAll(ct));
        public async Task<SalesOrderItemDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SalesOrderItemDto>(entity);
        }

        public async Task<Result<SalesOrderItemDto>> UpdateAsync(ulong id, SalesOrderItemRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SalesOrderItemDto>.Failure($"Sales order item {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SalesOrderItemDto>.Success(_mapper.Map<SalesOrderItemDto>(entity));
        }
    }
    public class SalesOrderPaymentScheduleService : ISalesOrderPaymentScheduleService
    {
        private readonly IRepository<Entities.SalesOrderPaymentSchedule> _repository;
        private readonly IMapper _mapper;

        public SalesOrderPaymentScheduleService(
            IRepository<Entities.SalesOrderPaymentSchedule> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SalesOrderPaymentScheduleDto>> CreateAsync(SalesOrderPaymentScheduleRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.SalesOrderPaymentSchedule>(request);
            await _repository.Add(entity, ct);
            return Result<SalesOrderPaymentScheduleDto>.Success(_mapper.Map<SalesOrderPaymentScheduleDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Sales order payment schedule {id} was not found.");
        }

        public async Task<List<SalesOrderPaymentScheduleDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<SalesOrderPaymentScheduleDto>>(await _repository.GetAll(ct));
        public async Task<SalesOrderPaymentScheduleDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<SalesOrderPaymentScheduleDto>(entity);
        }

        public async Task<Result<SalesOrderPaymentScheduleDto>> UpdateAsync(ulong id, SalesOrderPaymentScheduleRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<SalesOrderPaymentScheduleDto>.Failure($"Sales order payment schedule {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<SalesOrderPaymentScheduleDto>.Success(_mapper.Map<SalesOrderPaymentScheduleDto>(entity));
        }
    }
}
