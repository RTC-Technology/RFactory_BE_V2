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
    public class CountryService: ICountryService
    {
        private readonly IRepository<Entities.Country> _repository;
        private readonly IRepository<Entities.Province> _provinceRepository;
        private readonly IRepository<Entities.District> _districtRepository;
        private readonly IRepository<Entities.Ward> _wardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CountryService(
            IRepository<Entities.Country> repository,
            IRepository<Entities.Province> provinceRepository,
            IRepository<Entities.District> districtRepository,
            IRepository<Entities.Ward> wardRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CountryDto>> CreateAsync(CountryRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.CountryCode == request.CountryCode, ct);
            if (existing is not null)
            {
                return Result<CountryDto>.Failure($"Country '{request.CountryCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Country>(request);
            await _repository.Add(entity, ct);
            return Result<CountryDto>.Success(_mapper.Map<CountryDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Country {id} was not found.");
            }

            //var receiptId = (long)id;
            var provinces = await _provinceRepository.Where(p => p.CountryId == id, ct);
            var districts = await _districtRepository.Where(p => provinces.Select(d => d.Id).Contains(p.ProvinceId), ct);
            var wards = await _wardRepository.Where(p => districts.Select(d => d.Id).Contains(p.DistrictId), ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _wardRepository.DeleteRange(wards, token);
                await _districtRepository.DeleteRange(districts, token);
                await _provinceRepository.DeleteRange(provinces, token);

                await _repository.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<CountryDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<CountryDto>>(await _repository.GetAll(ct));
        public async Task<CountryDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<CountryDto>(entity);
        }

        public async Task<Result<CountryDto>> UpdateAsync(ulong id, CountryRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<CountryDto>.Failure($"Country {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.CountryCode == request.CountryCode, ct);
            if (existing is not null)
            {
                return Result<CountryDto>.Failure($"Country '{request.CountryCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<CountryDto>.Success(_mapper.Map<CountryDto>(entity));
        }
    }
    public class ProvinceService : IProvinceService
    {
        private readonly IRepository<Entities.Province> _repository;
        private readonly IRepository<Entities.District> _districtRepository;
        private readonly IRepository<Entities.Ward> _wardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProvinceService(
            IRepository<Entities.Province> repository,
            IRepository<Entities.District> districtRepository,
            IRepository<Entities.Ward> wardRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProvinceDto>> CreateAsync(ProvinceRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.ProvinceCode == request.ProvinceCode, ct);
            if (existing is not null)
            {
                return Result<ProvinceDto>.Failure($"Province '{request.ProvinceCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Province>(request);
            await _repository.Add(entity, ct);
            return Result<ProvinceDto>.Success(_mapper.Map<ProvinceDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Province {id} was not found.");
            }

            //var receiptId = (long)id;
            var districts = await _districtRepository.Where(p => p.ProvinceId == id, ct);
            var wards = await _wardRepository.Where(p => districts.Select(d => d.Id).Contains(p.DistrictId), ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _wardRepository.DeleteRange(wards, token);
                await _districtRepository.DeleteRange(districts, token);

                await _repository.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<ProvinceDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<ProvinceDto>>(await _repository.GetAll(ct));
        public async Task<ProvinceDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<ProvinceDto>(entity);
        }

        public async Task<Result<ProvinceDto>> UpdateAsync(ulong id, ProvinceRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<ProvinceDto>.Failure($"Province {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.ProvinceCode == request.ProvinceCode, ct);
            if (existing is not null)
            {
                return Result<ProvinceDto>.Failure($"Province '{request.ProvinceCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<ProvinceDto>.Success(_mapper.Map<ProvinceDto>(entity));
        }
    }
    public class DistrictService : IDistrictService
    {
        private readonly IRepository<Entities.District> _repository;
        private readonly IRepository<Entities.Ward> _wardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DistrictService(
            IRepository<Entities.District> repository,
            IRepository<Entities.Ward> wardRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _wardRepository = wardRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DistrictDto>> CreateAsync(DistrictRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.DistrictCode == request.DistrictCode, ct);
            if (existing is not null)
            {
                return Result<DistrictDto>.Failure($"District '{request.DistrictCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.District>(request);
            await _repository.Add(entity, ct);
            return Result<DistrictDto>.Success(_mapper.Map<DistrictDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Ward {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _wardRepository.Where(p => p.DistrictId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _wardRepository.DeleteRange(items, token);

                await _repository.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<DistrictDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<DistrictDto>>(await _repository.GetAll(ct));
        public async Task<DistrictDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<DistrictDto>(entity);
        }

        public async Task<Result<DistrictDto>> UpdateAsync(ulong id, DistrictRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<DistrictDto>.Failure($"District {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.DistrictCode == request.DistrictCode, ct);
            if (existing is not null)
            {
                return Result<DistrictDto>.Failure($"District '{request.DistrictCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<DistrictDto>.Success(_mapper.Map<DistrictDto>(entity));
        }
    }
    public class WardService : IWardService
    {
        private readonly IRepository<Entities.Ward> _repository;
        private readonly IMapper _mapper;

        public WardService(
            IRepository<Entities.Ward> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<WardDto>> CreateAsync(WardRequest request, CancellationToken ct = default)
        {
            var existing = await _repository.FirstOrDefault(p => p.WardCode == request.WardCode, ct);
            if (existing is not null)
            {
                return Result<WardDto>.Failure($"Ward '{request.WardCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Ward>(request);
            await _repository.Add(entity, ct);
            return Result<WardDto>.Success(_mapper.Map<WardDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Ward {id} was not found.");
        }

        public async Task<List<WardDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<WardDto>>(await _repository.GetAll(ct));
        public async Task<WardDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<WardDto>(entity);
        }

        public async Task<Result<WardDto>> UpdateAsync(ulong id, WardRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<WardDto>.Failure($"Ward {id} was not found.");
            }

            var existing = await _repository.FirstOrDefault(p => p.Id != id && p.WardCode == request.WardCode, ct);
            if (existing is not null)
            {
                return Result<WardDto>.Failure($"Ward '{request.WardCode}' already exists.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<WardDto>.Success(_mapper.Map<WardDto>(entity));
        }
    }


}
