using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.Inventory.DTOs;
using RFactory.Application.Modules.Inventory.Services;
using RFactory.Application.Modules.PurchaseOrder.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.PurchaseOrder.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IRepository<Entities.PurchaseOrder> _po;
        private readonly IRepository<Entities.PurchaseOrderDetail> _poDetail;
        private readonly IRepository<Entities.PurchaseOrderDeliverySchedule> _schedule;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseOrderService(
            IRepository<Entities.PurchaseOrder> po,
            IRepository<Entities.PurchaseOrderDetail> poDetail,
            IRepository<Entities.PurchaseOrderDeliverySchedule> schedule,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _po = po;
            _poDetail = poDetail;
            _schedule = schedule;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PurchaseOrderDto>> CreateAsync(PurchaseOrderRequest request, CancellationToken ct = default)
        {
            var existing = await _po.FirstOrDefault(t => t.Pono == request.Pono, ct);
            if (existing is not null)
            {
                return Result<PurchaseOrderDto>.Failure($"Purchase order '{request.Pono}' already exists.");
            }

            var entity = _mapper.Map<Entities.PurchaseOrder>(request);
            var lines = request.PurchaseOrderDetailRequests ?? new List<PurchaseOrderDetailRequest>();
            //var schedules = lines.SelectMany(x => x.PurchaseOrderDeliveryScheduleRequests ?? new List<PurchaseOrderDeliveryScheduleRequest>());

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // Two saves rather than one: the lines need the id the database generates for
                // the receipt, which is only known once the receipt is in.
                await _po.Add(entity, token);

                // 2. Map + insert Details
                var detailEntities = lines.Select(line => ToLineEntity(line, entity.Id)).ToList();
                await _poDetail.AddRange(detailEntities, token);

                // 3. Map + insert Schedules
                var scheduleEntities = new List<Entities.PurchaseOrderDeliverySchedule>();

                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    var detailEntity = detailEntities[i];

                    var schedules = line.PurchaseOrderDeliveryScheduleRequests ?? new List<PurchaseOrderDeliveryScheduleRequest>();

                    scheduleEntities.AddRange(schedules.Select(schedule => ToScheduleEntity(schedule, detailEntity.Id)));
                }

                if (scheduleEntities.Count > 0) await _schedule.AddRange(scheduleEntities, token);
                return Result<PurchaseOrderDto>.Success(_mapper.Map<PurchaseOrderDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _po.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Purchase order {id} was not found.");
            }

            //var receiptId = (long)id;
            var lines = await _poDetail.Where(p => p.PurchaseOrderId == id, ct);
            var schedules = await _schedule.Where(x => lines.Select(l => l.Id).Contains(x.PurchaseOrderDetailId), ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _schedule.DeleteRange(schedules, token);
                await _poDetail.DeleteRange(lines, token);
                await _po.Delete(entity, token);

                return Result.Success();
            }, ct);
        }

        public async Task<List<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<PurchaseOrderDto>>(await _po.GetAll(ct));

        public async Task<PurchaseOrderDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _po.GetById(id, ct);
            return entity is null ? null : _mapper.Map<PurchaseOrderDto>(entity);
        }

        public async Task<Result<PurchaseOrderDto>> UpdateAsync(ulong id, PurchaseOrderRequest request, CancellationToken ct = default)
        {
            var entity = await _po.GetById(id, ct);
            if (entity is null)
            {
                return Result<PurchaseOrderDto>.Failure($"Purchase order {id} was not found.");
            }

            var existing = await _po.FirstOrDefault(t => t.Id != id && t.Pono == request.Pono, ct);
            if (existing is not null)
            {
                return Result<PurchaseOrderDto>.Failure($"Purchase order '{request.Pono}' already exists.");
            }

            //var receiptId = (long)id;
            var stored = await _poDetail.Where(l => l.PurchaseOrderId == id, ct);

            var lines = request.PurchaseOrderDetailRequests;
            var keptIds = (lines ?? new List<PurchaseOrderDetailRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreign = keptIds.Where(lineId => stored.All(s => s.Id != lineId)).ToList();
            if (foreign.Count > 0)
            {
                return Result<PurchaseOrderDto>.Failure(
                    $"Line(s) {string.Join(", ", foreign)} do not belong to Purchase order {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _po.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (lines is not null)
                {
                    await _poDetail.DeleteRange(stored.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in lines.Where(l => l.Id != 0))
                    {
                        var target = stored.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _poDetail.Update(target, token);
                    }

                    await _poDetail.AddRange(lines.Where(l => l.Id == 0).Select(line => ToLineEntity(line, id)).ToList(), token);
                }

                return Result<PurchaseOrderDto>.Success(_mapper.Map<PurchaseOrderDto>(entity));
            }, ct);
        }

        private Entities.PurchaseOrderDetail ToLineEntity(PurchaseOrderDetailRequest line, ulong purchaseOrderId)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderDetail>(line);
            entity.PurchaseOrderId = purchaseOrderId;
            return entity;
        }

        private Entities.PurchaseOrderDeliverySchedule ToScheduleEntity(PurchaseOrderDeliveryScheduleRequest schedule, ulong purchaseOrderDetailId)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderDeliverySchedule>(schedule);
            entity.PurchaseOrderDetailId = purchaseOrderDetailId;

            return entity;
        }
    }
    //public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    //{
    //}
    //public class PurchaseOrderDeliveryScheduleService : IPurchaseOrderDeliveryScheduleService
    //{
    //}
}
