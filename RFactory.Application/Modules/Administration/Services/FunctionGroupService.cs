using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

public class FunctionGroupService : IFunctionGroupService
{
    private readonly IRepository<FunctionGroup> _repository;
    private readonly IMapper _mapper;

    public FunctionGroupService(IRepository<FunctionGroup> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<FunctionGroupDto>> GetAllAsync(CancellationToken ct = default)
    {
        var groups = await _repository.GetAll(ct);
        return _mapper.Map<List<FunctionGroupDto>>(groups);
    }

    public async Task<FunctionGroupDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var group = await _repository.GetById(id, ct);
        return group is null ? null : _mapper.Map<FunctionGroupDto>(group);
    }

    public async Task<Result<FunctionGroupDto>> CreateAsync(CreateFunctionGroupRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(g => g.Code == request.Code, ct);
        if (existing is not null)
        {
            return Result<FunctionGroupDto>.Failure($"Function group code '{request.Code}' already exists.");
        }

        var group = _mapper.Map<FunctionGroup>(request);
        await _repository.Add(group, ct);
        return Result<FunctionGroupDto>.Success(_mapper.Map<FunctionGroupDto>(group));
    }

    public async Task<Result<FunctionGroupDto>> UpdateAsync(ulong id, UpdateFunctionGroupRequest request, CancellationToken ct = default)
    {
        var group = await _repository.GetById(id, ct);
        if (group is null)
        {
            return Result<FunctionGroupDto>.Failure($"Function group {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(g => g.Id != id && g.Code == request.Code, ct);
        if (existing is not null)
        {
            return Result<FunctionGroupDto>.Failure($"Function group code '{request.Code}' already exists.");
        }

        _mapper.Map(request, group);
        await _repository.Update(group, ct);
        return Result<FunctionGroupDto>.Success(_mapper.Map<FunctionGroupDto>(group));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Function group {id} was not found.");
    }
}
