using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entities = RFactory.Infrastructure.Entities;

namespace RFactory.Application.Modules.MasterData.Services
{
    public class CurrencyService: ICurrencyService
    {
        private readonly IRepository<Entities.Currency> _repository;
        private readonly IMapper _mapper;

        public CurrencyService(
            IRepository<Entities.Currency> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<CurrencyDto>> CreateAsync(CurrencyRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.CurrencyCode == request.CurrencyCode, ct);
            if (existing is not null)
            {
                return Result<CurrencyDto>.Failure($"Currency '{request.CurrencyCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Currency>(request);
            await _repository.Add(entity, ct);
            return Result<CurrencyDto>.Success(_mapper.Map<CurrencyDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Currency {id} was not found.");
        }

        public async Task<List<CurrencyDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<CurrencyDto>>(await _repository.GetAll(ct));
        public async Task<CurrencyDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<CurrencyDto>(entity);
        }

        public async Task<Result<CurrencyDto>> UpdateAsync(ulong id, CurrencyRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<CurrencyDto>.Failure($"Currency {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.CurrencyCode == request.CurrencyCode, ct);
            if (existing is not null)
            {
                return Result<CurrencyDto>.Failure($"Currency '{request.CurrencyCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<CurrencyDto>.Success(_mapper.Map<CurrencyDto>(entity));
        }
    }
}
