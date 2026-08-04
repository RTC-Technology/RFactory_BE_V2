using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.MasterData.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IRepository<Organization> _repository;
    private readonly IMapper _mapper;

    public OrganizationService(IRepository<Organization> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<OrganizationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var organizations = await _repository.GetAll(ct);
        return _mapper.Map<List<OrganizationDto>>(organizations);
    }

    public async Task<OrganizationDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var organization = await _repository.GetById(id, ct);
        return organization is null ? null : _mapper.Map<OrganizationDto>(organization);
    }

    public async Task<Result<OrganizationDto>> CreateAsync(CreateOrganizationRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(o => o.OrganizationCode == request.OrganizationCode, ct);
        if (existing is not null)
        {
            return Result<OrganizationDto>.Failure($"Organization code '{request.OrganizationCode}' already exists.");
        }

        var organization = _mapper.Map<Organization>(request);
        await _repository.Add(organization, ct);
        return Result<OrganizationDto>.Success(_mapper.Map<OrganizationDto>(organization));
    }

    public async Task<Result<OrganizationDto>> UpdateAsync(ulong id, UpdateOrganizationRequest request, CancellationToken ct = default)
    {
        var organization = await _repository.GetById(id, ct);
        if (organization is null)
        {
            return Result<OrganizationDto>.Failure($"Organization {id} was not found.");
        }

        _mapper.Map(request, organization);
        await _repository.Update(organization, ct);
        return Result<OrganizationDto>.Success(_mapper.Map<OrganizationDto>(organization));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"Organization {id} was not found.");
    }
}
