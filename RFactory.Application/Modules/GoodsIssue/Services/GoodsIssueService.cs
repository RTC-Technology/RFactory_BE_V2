using AutoMapper;
using RFactory.Application.Modules.GoodsIssue.DTOs;
using RFactory.Application.Modules.GoodsReceipt.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.GoodsIssue.Services;

public class GoodsIssueService:IGoodsIssueService
{
    private readonly IRepository<Entities.GoodsIssue> _goodsIssue;
    private readonly IRepository<Entities.GoodsIssueDetail> _goodsIssueDetail;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GoodsIssueService(
        IRepository<Entities.GoodsIssue> goodsIssue,
        IRepository<Entities.GoodsIssueDetail> goodsIssueDetail,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _goodsIssue = goodsIssue;
        _goodsIssueDetail = goodsIssueDetail;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GoodsIssueDto>> CreateAsync(CreateGoodsIssueRequest request, CancellationToken ct = default)
    {
        try
        {
            var existing = await _goodsIssue.FirstOrDefault(t => t.IssueNo == request.IssueNo, ct);
            if (existing is not null)
            {
                return Result<GoodsIssueDto>.Failure($"Goods Issue '{request.IssueNo}' already exists.");
            }

            var entity = _mapper.Map<Entities.GoodsIssue>(request);
            var lines = request.GoodsIssueDetails ?? new List<GoodsIssueDetailRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                // Two saves rather than one: the lines need the id the database generates for
                // the receipt, which is only known once the receipt is in.
                await _goodsIssue.Add(entity, token);
                await _goodsIssueDetail.AddRange(
                    lines.Select(line => ToLineEntity(line, entity.Id)).ToList(), token);

                return Result<GoodsIssueDto>.Success(_mapper.Map<GoodsIssueDto>(entity));
            }, ct);
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _goodsIssue.GetById(id, ct);
        if (entity is null)
        {
            return Result.Failure($"Goods Issue {id} was not found.");
        }

        var receiptId = (long)id;
        var lines = await _goodsIssueDetail.Where(p => p.GoodsIssueId == receiptId, ct);

        // The lines belong to this receipt and nothing else, so they go with it instead of
        // blocking the delete — deleting is soft on both, and the pair moves together.
        return await _unitOfWork.ExecuteAsync<Result>(async token =>
        {
            await _goodsIssueDetail.DeleteRange(lines, token);
            await _goodsIssue.Delete(entity, token);
            return Result.Success();
        }, ct);
    }

    public async Task<List<GoodsIssueDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<GoodsIssueDto>>(await _goodsIssue.GetAll(ct));

    public async Task<GoodsIssueDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _goodsIssue.GetById(id, ct);
        return entity is null ? null : _mapper.Map<GoodsIssueDto>(entity);
    }

    public async Task<Result<GoodsIssueDto>> UpdateAsync(ulong id, UpdateGoodsIssueRequest request, CancellationToken ct = default)
    {
        var entity = await _goodsIssue.GetById(id, ct);
        if (entity is null)
        {
            return Result<GoodsIssueDto>.Failure($"Goods Issue {id} was not found.");
        }

        var existing = await _goodsIssue.FirstOrDefault(
            t => t.Id != id && t.IssueNo == request.IssueNo, ct);
        if (existing is not null)
        {
            return Result<GoodsIssueDto>.Failure($"Goods Issue '{request.IssueNo}' already exists.");
        }

        var receiptId = (long)id;
        var stored = await _goodsIssueDetail.Where(l => l.GoodsIssueId == receiptId, ct);

        var lines = request.GoodsIssueDetails;
        var keptIds = (lines ?? new List<GoodsIssueDetailRequest>())
            .Where(line => line.Id != 0)
            .Select(line => line.Id)
            .ToHashSet();

        // The list replaces the whole set, so an id from another receipt would be edited
        // here and dropped from where it belongs. Reject the payload instead.
        var foreign = keptIds.Where(lineId => stored.All(s => s.Id != lineId)).ToList();
        if (foreign.Count > 0)
        {
            return Result<GoodsIssueDto>.Failure(
                $"Line(s) {string.Join(", ", foreign)} do not belong to Goods Receipt {id}.");
        }

        _mapper.Map(request, entity);

        return await _unitOfWork.ExecuteAsync(async token =>
        {
            await _goodsIssue.Update(entity, token);

            // A null list means the caller is editing the header only; an empty one means
            // the receipt really has no lines left.
            if (lines is not null)
            {
                await _goodsIssueDetail.DeleteRange(
                    stored.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                foreach (var line in lines.Where(l => l.Id != 0))
                {
                    var target = stored.First(s => s.Id == line.Id);
                    _mapper.Map(line, target);
                    await _goodsIssueDetail.Update(target, token);
                }

                await _goodsIssueDetail.AddRange(
                    lines.Where(l => l.Id == 0).Select(line => ToLineEntity(line, id)).ToList(), token);
            }

            return Result<GoodsIssueDto>.Success(_mapper.Map<GoodsIssueDto>(entity));
        }, ct);
    }

    private Entities.GoodsIssueDetail ToLineEntity(GoodsIssueDetailRequest line, ulong receiptId)
    {
        var entity = _mapper.Map<Entities.GoodsIssueDetail>(line);
        entity.GoodsIssueId = (long)receiptId;
        return entity;
    }
}

public class GoodsIssueDetailService : IGoodsIssueDetailService
{
    private readonly IRepository<Entities.GoodsIssueDetail> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GoodsIssueDetailService(
        IRepository<Entities.GoodsIssueDetail> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GoodsIssueDetailDto>> CreateAsync(GoodsIssueDetailRequest request, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Entities.GoodsIssueDetail>(request);
        await _repository.Add(entity, ct);
        return Result<GoodsIssueDetailDto>.Success(_mapper.Map<GoodsIssueDetailDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Goods Issue Detail line {id} was not found.");
    }

    /// <summary>Lines of one receipt, or every line when <paramref name="receiptId"/> is null.</summary>
    public async Task<List<GoodsIssueDetailDto>> GetAllAsync(long? issueId, CancellationToken ct = default)
    {
        var entities = issueId.HasValue
            ? await _repository.Where(x => x.GoodsIssueId == issueId.Value, ct)
            : await _repository.GetAll(ct);

        return _mapper.Map<List<GoodsIssueDetailDto>>(entities);
    }

    public async Task<GoodsIssueDetailDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<GoodsIssueDetailDto>(entity);
    }

    public async Task<Result<GoodsIssueDetailDto>> UpdateAsync(ulong id, GoodsIssueDetailRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
        {
            return Result<GoodsIssueDetailDto>.Failure($"Goods Issue Detail line {id} was not found.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<GoodsIssueDetailDto>.Success(_mapper.Map<GoodsIssueDetailDto>(entity));
    }

   
}

