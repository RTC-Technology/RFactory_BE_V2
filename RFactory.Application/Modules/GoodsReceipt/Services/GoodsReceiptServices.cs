using AutoMapper;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Infrastructure.Entities;
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



    public class GoodsReceiptDetailServices : IGoodsReceiptDetailServices
    {
        //private readonly IRepository<Entities.GoodsReceipt> _goodsReceipt;
        private readonly IRepository<Entities.GoodsReceiptDetail> _repository;
        private readonly IMapper _mapper;

        public GoodsReceiptDetailServices(
            //IRepository<Entities.GoodsReceipt> goodsReceipt,
            IRepository<Entities.GoodsReceiptDetail> repository,
            IMapper mapper)
        {
            //_goodsReceipt = goodsReceipt;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<GoodsReceiptDetailDto>> CreateAsync(CreateGoodsReceiptDetailRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.GoodsReceiptDetail>(request);
            await _repository.Add(entity, ct);
            return Result<GoodsReceiptDetailDto>.Success(_mapper.Map<GoodsReceiptDetailDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Goods Receipt Detail line {id} was not found.");
        }

        public async Task<List<GoodsReceiptDetailDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<GoodsReceiptDetailDto>>(await _repository.GetAll(ct));

        public async Task<GoodsReceiptDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<GoodsReceiptDetailDto>(entity);
        }

        public async Task<Result<GoodsReceiptDetailDto>> UpdateAsync(ulong id, UpdatesGoodsReceiptDetailRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<GoodsReceiptDetailDto>.Failure($"Goods Receipt Detail line {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<GoodsReceiptDetailDto>.Success(_mapper.Map<GoodsReceiptDetailDto>(entity));
        }
    }
}
