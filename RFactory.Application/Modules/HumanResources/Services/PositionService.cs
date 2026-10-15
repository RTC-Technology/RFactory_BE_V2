using AutoMapper;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.HumanResources.Services;

public class PositionService : IPositionService
{
    private readonly IRepository<Position> _repository;
    private readonly IMapper _mapper;

    public PositionService(IRepository<Position> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<PositionDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<PositionDto>>(await _repository.GetAll(ct));

    public async Task<PositionDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<PositionDto>(entity);
    }

    public async Task<Result<PositionDto>> CreateAsync(CreatePositionRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(p => p.PositionCode == request.PositionCode, ct);
        if (existing is not null)
            return Result<PositionDto>.Failure($"Position code '{request.PositionCode}' already exists.");

        var entity = _mapper.Map<Position>(request);
        await _repository.Add(entity, ct);
        return Result<PositionDto>.Success(_mapper.Map<PositionDto>(entity));
    }

    public async Task<Result<PositionDto>> UpdateAsync(ulong id, UpdatePositionRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
            return Result<PositionDto>.Failure($"Position {id} was not found.");

        var clash = await _repository.FirstOrDefault(p => p.Id != id && p.PositionCode == request.PositionCode, ct);
        if (clash is not null)
            return Result<PositionDto>.Failure($"Position code '{request.PositionCode}' already exists.");

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<PositionDto>.Success(_mapper.Map<PositionDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Position {id} was not found.");
    }
}
