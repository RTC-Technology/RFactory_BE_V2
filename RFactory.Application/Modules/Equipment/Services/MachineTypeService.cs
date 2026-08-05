using AutoMapper;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Equipment.Services;

public class MachineTypeService : IMachineTypeService
{
    private readonly IRepository<MachineType> _repository;
    private readonly IMapper _mapper;

    public MachineTypeService(IRepository<MachineType> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<MachineTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var machineTypes = await _repository.GetAll(ct);
        return _mapper.Map<List<MachineTypeDto>>(machineTypes);
    }

    public async Task<MachineTypeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var machineType = await _repository.GetById(id, ct);
        return machineType is null ? null : _mapper.Map<MachineTypeDto>(machineType);
    }

    public async Task<Result<MachineTypeDto>> CreateAsync(CreateMachineTypeRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(m => m.MachineTypeCode == request.MachineTypeCode, ct);
        if (existing is not null)
        {
            return Result<MachineTypeDto>.Failure($"Machine type code '{request.MachineTypeCode}' already exists.");
        }

        var machineType = _mapper.Map<MachineType>(request);
        await _repository.Add(machineType, ct);
        return Result<MachineTypeDto>.Success(_mapper.Map<MachineTypeDto>(machineType));
    }

    public async Task<Result<MachineTypeDto>> UpdateAsync(ulong id, UpdateMachineTypeRequest request, CancellationToken ct = default)
    {
        var machineType = await _repository.GetById(id, ct);
        if (machineType is null)
        {
            return Result<MachineTypeDto>.Failure($"Machine type {id} was not found.");
        }

        _mapper.Map(request, machineType);
        await _repository.Update(machineType, ct);
        return Result<MachineTypeDto>.Success(_mapper.Map<MachineTypeDto>(machineType));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Machine type {id} was not found.");
    }
}
