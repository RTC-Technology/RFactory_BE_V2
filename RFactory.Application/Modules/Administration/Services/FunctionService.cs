using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Constants;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

public class FunctionService : IFunctionService
{
    private readonly IRepository<Function> _repository;
    private readonly IRepository<FunctionGroup> _groups;
    private readonly IMapper _mapper;

    public FunctionService(
        IRepository<Function> repository,
        IRepository<FunctionGroup> groups,
        IMapper mapper)
    {
        _repository = repository;
        _groups = groups;
        _mapper = mapper;
    }

    public async Task<List<FunctionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var functions = await _repository.GetAll(ct);
        return _mapper.Map<List<FunctionDto>>(functions);
    }

    public async Task<FunctionDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var function = await _repository.GetById(id, ct);
        return function is null ? null : _mapper.Map<FunctionDto>(function);
    }

    public async Task<Result<FunctionDto>> CreateAsync(CreateFunctionRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(f => f.FunctionCode == request.FunctionCode, ct);
        if (existing is not null)
        {
            return Result<FunctionDto>.Failure($"Function code '{request.FunctionCode}' already exists.");
        }

        var function = _mapper.Map<Function>(request);
        await _repository.Add(function, ct);
        return Result<FunctionDto>.Success(_mapper.Map<FunctionDto>(function));
    }

    public async Task<Result<FunctionDto>> UpdateAsync(ulong id, UpdateFunctionRequest request, CancellationToken ct = default)
    {
        var function = await _repository.GetById(id, ct);
        if (function is null)
        {
            return Result<FunctionDto>.Failure($"Function {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(f => f.Id != id && f.FunctionCode == request.FunctionCode, ct);
        if (existing is not null)
        {
            return Result<FunctionDto>.Failure($"Function code '{request.FunctionCode}' already exists.");
        }

        _mapper.Map(request, function);
        await _repository.Update(function, ct);
        return Result<FunctionDto>.Success(_mapper.Map<FunctionDto>(function));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Function {id} was not found.");
    }

    /// <summary>
    /// Writes any catalogue entry the database is missing.
    ///
    /// Additive only: existing rows keep their names and ids, so group assignments and
    /// every UserGroupRightDistribution pointing at them survive. Codes the application
    /// no longer enforces are left alone too — deciding those are dead is a judgement the
    /// admin makes, not something a sync should do behind their back.
    /// </summary>
    public async Task<Result<PermissionSyncResult>> SyncCatalogAsync(CancellationToken ct = default)
    {
        var existingGroups = await _groups.GetAll(ct);
        var groupByCode = existingGroups
            .Where(g => !string.IsNullOrWhiteSpace(g.Code))
            .ToDictionary(g => g.Code, g => g, StringComparer.OrdinalIgnoreCase);

        var newGroups = PermissionCatalog.Groups
            .Where(spec => !groupByCode.ContainsKey(spec.Code))
            .Select(spec => new FunctionGroup { Code = spec.Code, Name = spec.Name })
            .ToList();

        await _groups.AddRange(newGroups, ct);
        foreach (var group in newGroups)
        {
            groupByCode[group.Code] = group;
        }

        var existingCodes = (await _repository.GetAll(ct))
            .Select(f => f.FunctionCode)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newFunctions = new List<Function>();
        foreach (var spec in PermissionCatalog.Groups)
        {
            // Populated either from the pre-existing row or from the batch just inserted,
            // whose ids EF filled in on save.
            var groupId = (int)groupByCode[spec.Code].Id;

            newFunctions.AddRange(spec.Permissions
                .Where(permission => !existingCodes.Contains(permission.Code))
                .Select(permission => new Function
                {
                    FunctionCode = permission.Code,
                    FunctionName = permission.Name,
                    FunctionGroupId = groupId,
                }));
        }

        await _repository.AddRange(newFunctions, ct);

        return Result<PermissionSyncResult>.Success(
            new PermissionSyncResult(newGroups.Count, newFunctions.Count, PermissionCatalog.AllCodes.Count));
    }
}
