using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

public class FunctionService : IFunctionService
{
    private readonly IRepository<Function> _repository;
    private readonly IMapper _mapper;

    public FunctionService(IRepository<Function> repository, IMapper mapper)
    {
        _repository = repository;
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
}
