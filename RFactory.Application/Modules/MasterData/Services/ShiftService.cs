using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class ShiftService : IShiftService
{
    private readonly IRepository<Shift> _repository;
    private readonly IRepository<ShiftBreak> _breaks;
    private readonly IMapper _mapper;

    public ShiftService(IRepository<Shift> repository, IRepository<ShiftBreak> breaks, IMapper mapper)
    {
        _repository = repository;
        _breaks = breaks;
        _mapper = mapper;
    }

    public async Task<List<ShiftDto>> GetAllAsync(CancellationToken ct = default)
    {
        var shifts = await _repository.GetAll(ct);
        return _mapper.Map<List<ShiftDto>>(shifts);
    }

    public async Task<ShiftDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var shift = await _repository.GetById(id, ct);
        return shift is null ? null : _mapper.Map<ShiftDto>(shift);
    }

    public async Task<Result<ShiftDto>> CreateAsync(CreateShiftRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(s => s.ShiftCode == request.ShiftCode, ct);
        if (existing is not null)
        {
            return Result<ShiftDto>.Failure($"Shift code '{request.ShiftCode}' already exists.");
        }

        var shift = _mapper.Map<Shift>(request);
        await _repository.Add(shift, ct);
        return Result<ShiftDto>.Success(_mapper.Map<ShiftDto>(shift));
    }

    public async Task<Result<ShiftDto>> UpdateAsync(ulong id, UpdateShiftRequest request, CancellationToken ct = default)
    {
        var shift = await _repository.GetById(id, ct);
        if (shift is null)
        {
            return Result<ShiftDto>.Failure($"Shift {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(s => s.Id != id && s.ShiftCode == request.ShiftCode, ct);
        if (existing is not null)
        {
            return Result<ShiftDto>.Failure($"Shift code '{request.ShiftCode}' already exists.");
        }

        _mapper.Map(request, shift);
        await _repository.Update(shift, ct);
        return Result<ShiftDto>.Success(_mapper.Map<ShiftDto>(shift));
    }

    /// <summary>
    /// Refuses while breaks still point here. Delete is a non-cascading soft delete, so
    /// those breaks would keep a ShiftId that no longer reads and drop out of every list.
    /// </summary>
    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var shift = await _repository.GetById(id, ct);
        if (shift is null)
        {
            return Result.Failure($"Shift {id} was not found.");
        }

        var shiftId = (long)id;
        var attached = await _breaks.Where(b => b.ShiftId == shiftId, ct);
        if (attached.Count > 0)
        {
            return Result.Failure($"Shift {id} still has {attached.Count} break(s). Remove them first.");
        }

        await _repository.Delete(shift, ct);
        return Result.Success();
    }
}

public class ShiftBreakService : IShiftBreakService
{
    private readonly IRepository<ShiftBreak> _repository;
    private readonly IMapper _mapper;

    public ShiftBreakService(IRepository<ShiftBreak> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ShiftBreakDto>> GetAllAsync(CancellationToken ct = default)
    {
        var breaks = await _repository.GetAll(ct);
        return _mapper.Map<List<ShiftBreakDto>>(breaks);
    }

    public async Task<ShiftBreakDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var item = await _repository.GetById(id, ct);
        return item is null ? null : _mapper.Map<ShiftBreakDto>(item);
    }

    public async Task<Result<ShiftBreakDto>> CreateAsync(CreateShiftBreakRequest request, CancellationToken ct = default)
    {
        var item = _mapper.Map<ShiftBreak>(request);
        await _repository.Add(item, ct);
        return Result<ShiftBreakDto>.Success(_mapper.Map<ShiftBreakDto>(item));
    }

    public async Task<Result<ShiftBreakDto>> UpdateAsync(ulong id, UpdateShiftBreakRequest request, CancellationToken ct = default)
    {
        var item = await _repository.GetById(id, ct);
        if (item is null)
        {
            return Result<ShiftBreakDto>.Failure($"Shift break {id} was not found.");
        }

        _mapper.Map(request, item);
        await _repository.Update(item, ct);
        return Result<ShiftBreakDto>.Success(_mapper.Map<ShiftBreakDto>(item));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Shift break {id} was not found.");
    }
}
