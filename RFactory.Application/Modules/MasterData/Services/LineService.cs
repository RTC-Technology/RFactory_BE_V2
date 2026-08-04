using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class LineService : ILineService
{
    private readonly IRepository<Line> _repository;
    private readonly IMapper _mapper;

    public LineService(IRepository<Line> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<LineDto>> GetAllAsync(CancellationToken ct = default)
    {
        var lines = await _repository.GetAll(ct);
        return _mapper.Map<List<LineDto>>(lines);
    }

    public async Task<LineDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var line = await _repository.GetById(id, ct);
        return line is null ? null : _mapper.Map<LineDto>(line);
    }

    public async Task<Result<LineDto>> CreateAsync(CreateLineRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(l => l.LineCode == request.LineCode, ct);
        if (existing is not null)
        {
            return Result<LineDto>.Failure($"Line code '{request.LineCode}' already exists.");
        }

        var line = _mapper.Map<Line>(request);
        await _repository.Add(line, ct);
        return Result<LineDto>.Success(_mapper.Map<LineDto>(line));
    }

    public async Task<Result<LineDto>> UpdateAsync(ulong id, UpdateLineRequest request, CancellationToken ct = default)
    {
        var line = await _repository.GetById(id, ct);
        if (line is null)
        {
            return Result<LineDto>.Failure($"Line {id} was not found.");
        }

        _mapper.Map(request, line);
        await _repository.Update(line, ct);
        return Result<LineDto>.Success(_mapper.Map<LineDto>(line));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Line {id} was not found.");
    }
}
