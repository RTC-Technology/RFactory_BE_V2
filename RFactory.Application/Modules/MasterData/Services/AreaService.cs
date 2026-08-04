using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class AreaService : IAreaService
{
    private readonly IRepository<Area> _repository;
    private readonly IMapper _mapper;

    public AreaService(IRepository<Area> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<AreaDto>> GetAllAsync(CancellationToken ct = default)
    {
        var areas = await _repository.GetAll(ct);
        return _mapper.Map<List<AreaDto>>(areas);
    }

    public async Task<AreaDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var area = await _repository.GetById(id, ct);
        return area is null ? null : _mapper.Map<AreaDto>(area);
    }

    public async Task<Result<AreaDto>> CreateAsync(CreateAreaRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(a => a.AreaCode == request.AreaCode, ct);
        if (existing is not null)
        {
            return Result<AreaDto>.Failure($"Area code '{request.AreaCode}' already exists.");
        }

        var area = _mapper.Map<Area>(request);
        await _repository.Add(area, ct);
        return Result<AreaDto>.Success(_mapper.Map<AreaDto>(area));
    }

    public async Task<Result<AreaDto>> UpdateAsync(ulong id, UpdateAreaRequest request, CancellationToken ct = default)
    {
        var area = await _repository.GetById(id, ct);
        if (area is null)
        {
            return Result<AreaDto>.Failure($"Area {id} was not found.");
        }

        _mapper.Map(request, area);
        await _repository.Update(area, ct);
        return Result<AreaDto>.Success(_mapper.Map<AreaDto>(area));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Area {id} was not found.");
    }
}
