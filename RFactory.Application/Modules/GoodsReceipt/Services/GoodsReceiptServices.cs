using AutoMapper;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.GoodsReceipt.Services
{
    public class GoodsReceiptServices: IGoodsReceiptServices    
    {
        private readonly IRepository<Entities.GoodsReceipt> _goodsReceipt;
        private readonly IRepository<Entities.GoodsReceiptDetail> _goodsReceiptDetail;
        private readonly IMapper _mapper;

        public GoodsReceiptServices(
            IRepository<Entities.GoodsReceipt> goodsReceipt,
            IRepository<Entities.GoodsReceiptDetail> goodsReceiptDetail,
            IMapper mapper)
        {
            _goodsReceipt = goodsReceipt;
            _goodsReceiptDetail = goodsReceiptDetail;
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
            await _goodsReceipt.Add(entity, ct);
            return Result<GoodsReceiptDto>.Success(_mapper.Map<GoodsReceiptDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _goodsReceipt.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Goods Receipt {id} was not found.");
            }

            var typeId = (long)id;
            var inUse = await _goodsReceiptDetail.Where(p => p.GoodsReceiptId == typeId, ct);
            if (inUse.Count > 0)
            {
                return Result.Failure($"Goods Receipt {id} is still used by {inUse.Count} Goods Receipt Detail(s).");
            }

            await _goodsReceipt.Delete(entity, ct);
            return Result.Success();
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

            _mapper.Map(request, entity);
            await _goodsReceipt.Update(entity, ct);
            return Result<GoodsReceiptDto>.Success(_mapper.Map<GoodsReceiptDto>(entity));
        }
    }
}
