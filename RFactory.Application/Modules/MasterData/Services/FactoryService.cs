using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class FactoryService : IFactoryService
{
    private readonly IRepository<Factory> _repository;
    private readonly IMapper _mapper;

    public FactoryService(IRepository<Factory> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<FactoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var factories = await _repository.GetAll(ct);
        return _mapper.Map<List<FactoryDto>>(factories);
    }

    public async Task<FactoryDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var factory = await _repository.GetById(id, ct);
        return factory is null ? null : _mapper.Map<FactoryDto>(factory);
    }

    public async Task<Result<FactoryDto>> CreateAsync(CreateFactoryRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(f => f.FactoryCode == request.FactoryCode, ct);
        if (existing is not null)
        {
            return Result<FactoryDto>.Failure($"Factory code '{request.FactoryCode}' already exists.");
        }

        var factory = _mapper.Map<Factory>(request);
        await _repository.Add(factory, ct);
        return Result<FactoryDto>.Success(_mapper.Map<FactoryDto>(factory));
    }

    public async Task<Result<FactoryDto>> UpdateAsync(ulong id, UpdateFactoryRequest request, CancellationToken ct = default)
    {
        var factory = await _repository.GetById(id, ct);
        if (factory is null)
        {
            return Result<FactoryDto>.Failure($"Factory {id} was not found.");
        }

        _mapper.Map(request, factory);
        await _repository.Update(factory, ct);
        return Result<FactoryDto>.Success(_mapper.Map<FactoryDto>(factory));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Factory {id} was not found.");
    }
}
