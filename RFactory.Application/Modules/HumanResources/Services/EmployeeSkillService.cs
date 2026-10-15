using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RFactory.Application.Modules.HumanResources.DTOs;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.HumanResources.Services;

public class EmployeeSkillService : IEmployeeSkillService
{
    private readonly IRepository<EmployeeSkill> _repository;
    private readonly RFactoryContext _context;
    private readonly IMapper _mapper;

    public EmployeeSkillService(IRepository<EmployeeSkill> repository, RFactoryContext context, IMapper mapper)
    {
        _repository = repository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EmployeeSkillDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await BuildQuery().ToListAsync(ct);
    }

    public async Task<List<EmployeeSkillDto>> GetByEmployeeAsync(ulong employeeId, CancellationToken ct = default)
    {
        return await BuildQuery().Where(dto => dto.EmployeeId == employeeId).ToListAsync(ct);
    }

    public async Task<EmployeeSkillDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        return await BuildQuery().FirstOrDefaultAsync(dto => dto.Id == id, ct);
    }

    public async Task<Result<EmployeeSkillDto>> CreateAsync(CreateEmployeeSkillRequest request, CancellationToken ct = default)
    {
        // Prevent duplicates: same employee + same skill.
        var existing = await _repository.FirstOrDefault(
            es => es.EmployeeId == request.EmployeeId && es.SkillId == request.SkillId, ct);
        if (existing is not null)
            return Result<EmployeeSkillDto>.Failure("This employee already has that skill assigned.");

        var entity = _mapper.Map<EmployeeSkill>(request);
        await _repository.Add(entity, ct);

        var dto = await GetByIdAsync(entity.Id, ct);
        return Result<EmployeeSkillDto>.Success(dto!);
    }

    public async Task<Result<EmployeeSkillDto>> UpdateAsync(ulong id, UpdateEmployeeSkillRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetById(id, ct);
        if (entity is null)
            return Result<EmployeeSkillDto>.Failure($"Employee skill {id} was not found.");

        // If the skill is being changed, check for duplicates.
        if (entity.SkillId != request.SkillId)
        {
            var clash = await _repository.FirstOrDefault(
                es => es.Id != id && es.EmployeeId == entity.EmployeeId && es.SkillId == request.SkillId, ct);
            if (clash is not null)
                return Result<EmployeeSkillDto>.Failure("This employee already has that skill assigned.");
        }

        _mapper.Map(request, entity);
        await _repository.Update(entity, ct);

        var dto = await GetByIdAsync(id, ct);
        return Result<EmployeeSkillDto>.Success(dto!);
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Employee skill {id} was not found.");
    }

    // ─── Private helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Joins EmployeeSkill → Skill to produce denormalised DTOs in one query.
    /// </summary>
    private IQueryable<EmployeeSkillDto> BuildQuery()
    {
        return from es in _context.EmployeeSkills
               join s in _context.Skills on es.SkillId equals s.Id into sj
               from s in sj.DefaultIfEmpty()
               where !es.IsDeleted
               select new EmployeeSkillDto
               {
                   Id = es.Id,
                   EmployeeId = es.EmployeeId,
                   SkillId = es.SkillId,
                   SkillCode = s != null ? s.SkillCode : null,
                   SkillName = s != null ? s.SkillName : null,
                   SkillCategory = s != null ? s.SkillCategory : null,
                   ProficiencyLevel = es.ProficiencyLevel,
                   AcquiredDate = es.AcquiredDate,
                   Notes = es.Notes,
               };
    }
}
