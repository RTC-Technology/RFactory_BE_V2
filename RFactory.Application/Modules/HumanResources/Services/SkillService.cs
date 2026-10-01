using AutoMapper;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.HumanResources.Services;

public class SkillService : ISkillService
{
    private readonly IRepository<Skill> _repository;
    private readonly IMapper _mapper;

    public SkillService(IRepository<Skill> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<SkillDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<SkillDto>>(await _repository.GetAll(ct));

    public async Task<SkillDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<SkillDto>(entity);
    }

    public async Task<Result<SkillDto>> CreateAsync(CreateSkillRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(s => s.SkillCode == request.SkillCode, ct);
        if (existing is not null)
            return Result<SkillDto>.Failure($"Skill code '{request.SkillCode}' already exists.");

        var entity = _mapper.Map<Skill>(request);
        await _repository.Add(entity, ct);
        return Result<SkillDto>.Success(_mapper.Map<SkillDto>(entity));
    }

    public async Task<Result<SkillDto>> UpdateAsync(ulong id, UpdateSkillRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
            return Result<SkillDto>.Failure($"Skill {id} was not found.");

        var clash = await _repository.FirstOrDefault(s => s.Id != id && s.SkillCode == request.SkillCode, ct);
        if (clash is not null)
            return Result<SkillDto>.Failure($"Skill code '{request.SkillCode}' already exists.");

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);
        return Result<SkillDto>.Success(_mapper.Map<SkillDto>(entity));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Skill {id} was not found.");
    }
}
