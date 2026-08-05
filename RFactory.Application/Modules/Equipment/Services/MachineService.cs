using AutoMapper;
using RFactory.Application.Modules.Equipment.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Equipment.Services;

public class MachineService : IMachineService
{
    private readonly IRepository<Machine> _repository;
    private readonly IMapper _mapper;

    public MachineService(IRepository<Machine> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<MachineDto>> GetAllAsync(CancellationToken ct = default)
    {
        var machines = await _repository.GetAll(ct);
        return _mapper.Map<List<MachineDto>>(machines);
    }

    public async Task<MachineDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var machine = await _repository.GetById(id, ct);
        return machine is null ? null : _mapper.Map<MachineDto>(machine);
    }

    public async Task<Result<MachineDto>> CreateAsync(CreateMachineRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(m => m.MachineCode == request.MachineCode, ct);
        if (existing is not null)
        {
            return Result<MachineDto>.Failure($"Machine code '{request.MachineCode}' already exists.");
        }

        var machine = _mapper.Map<Machine>(request);
        await _repository.Add(machine, ct);
        return Result<MachineDto>.Success(_mapper.Map<MachineDto>(machine));
    }

    public async Task<Result<MachineDto>> UpdateAsync(ulong id, UpdateMachineRequest request, CancellationToken ct = default)
    {
        var machine = await _repository.GetById(id, ct);
        if (machine is null)
        {
            return Result<MachineDto>.Failure($"Machine {id} was not found.");
        }

        _mapper.Map(request, machine);
        await _repository.Update(machine, ct);
        return Result<MachineDto>.Success(_mapper.Map<MachineDto>(machine));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Machine {id} was not found.");
    }
}
