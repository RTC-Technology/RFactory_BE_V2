using AutoMapper;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.GoodsReceipt.Services;

/// <summary>
/// Line-level CRUD, kept for callers that address a line directly. The receipt screen does
/// not write through here — it sends the lines nested in the receipt so both land in one
/// transaction, see <see cref="GoodsReceiptService"/>.
/// </summary>
public class GoodsReceiptDetailService : IGoodsReceiptDetailService
{
    private readonly IRepository<Entities.GoodsReceiptDetail> _repository;
    private readonly IMapper _mapper;

    public GoodsReceiptDetailService(
        IRepository<Entities.GoodsReceiptDetail> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GoodsReceiptDetailDto>> CreateAsync(CreateGoodsReceiptDetailRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.GoodsReceiptDetail>(request);
        await _repository.Add(entity, ct);
        return Result<GoodsReceiptDetailDto>.Success(_mapper.Map<GoodsReceiptDetailDto>(entity));
    }

    public async Task<Result<List<GoodsReceiptDetailDto>>> CreateRangeAsync(List<CreateGoodsReceiptDetailRequest> requests, CancellationToken ct = default)
    {
        var entities = _mapper.Map<List<Entities.GoodsReceiptDetail>>(requests);
        await _repository.AddRange(entities, ct);
        return Result<List<GoodsReceiptDetailDto>>.Success(_mapper.Map<List<GoodsReceiptDetailDto>>(entities));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Goods Receipt Detail line {id} was not found.");
    }

    /// <summary>Lines of one receipt, or every line when <paramref name="receiptId"/> is null.</summary>
    public async Task<List<GoodsReceiptDetailDto>> GetAllAsync(long? receiptId, CancellationToken ct = default)
    {
        var entities = receiptId.HasValue
            ? await _repository.Where(x => x.GoodsReceiptId == receiptId.Value, ct)
            : await _repository.GetAll(ct);

        return _mapper.Map<List<GoodsReceiptDetailDto>>(entities);
    }

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

    public async Task<Result<List<GoodsReceiptDetailDto>>> UpdateRangeAsync(List<UpdatesGoodsReceiptDetailRequest> requests, CancellationToken ct = default)
    {
        var entities = _mapper.Map<List<Entities.GoodsReceiptDetail>>(requests);
        await _repository.DeleteRange(entities, ct);

        requests.ForEach(x => x.Id = 0);
        var replacements = _mapper.Map<List<Entities.GoodsReceiptDetail>>(requests);

        await _repository.AddRange(replacements, ct);
        return Result<List<GoodsReceiptDetailDto>>.Success(_mapper.Map<List<GoodsReceiptDetailDto>>(replacements));
    }
}
