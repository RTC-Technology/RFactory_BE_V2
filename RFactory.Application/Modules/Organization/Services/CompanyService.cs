using AutoMapper;
using RFactory.Application.Modules.DeliveryNote.DTOs;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Application.Modules.Product.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.Organizations.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Entities.Company> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompanyService(
            IRepository<Entities.Company> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CompanyDto>> CreateAsync(CompanyRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.CompanyCode == request.CompanyCode, ct);
            if (existing is not null)
            {
                return Result<CompanyDto>.Failure($"Company code '{request.CompanyCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Company>(request);
            await _repository.Add(entity, ct);
            return Result<CompanyDto>.Success(_mapper.Map<CompanyDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Company {id} was not found.");
        }

        public async Task<List<CompanyDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<CompanyDto>>(await _repository.GetAll(ct));
        public async Task<CompanyDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<CompanyDto>(entity);
        }

        public async Task<Result<CompanyDto>> UpdateAsync(ulong id, CompanyRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<CompanyDto>.Failure($"Company {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.CompanyCode == request.CompanyCode, ct);
            if (existing is not null)
            {
                return Result<CompanyDto>.Failure($"Company code '{request.CompanyCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<CompanyDto>.Success(_mapper.Map<CompanyDto>(entity));
        }
    }
}
