using AutoMapper;
using RFactory.Application.Modules.Organizations.DTOs;
using RFactory.Infrastructure.Entities;
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
    public class ProductionTeamService : IProductionTeamService
    {
        private readonly IRepository<Entities.ProductionTeam> _repo;
        private readonly IRepository<Entities.ProductionTeamEmployee> _employee;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductionTeamService(
            IRepository<Entities.ProductionTeam> repo,
            IRepository<Entities.ProductionTeamEmployee> employee,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _employee = employee;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductionTeamDto>> CreateAsync(ProductionTeamRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.TeamCode == request.TeamCode, ct);
            if (existing is not null)
            {
                return Result<ProductionTeamDto>.Failure($"Production team '{request.TeamCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.ProductionTeam>(request);
            var lines = request.Employees ?? new List<ProductionTeamEmployeeRequest>();

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                var detailEntities = lines.Select(line => ToLineEntity(line, entity.Id)).ToList();
                await _employee.AddRange(detailEntities, token);
                

                return Result<ProductionTeamDto>.Success(_mapper.Map<ProductionTeamDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Production team {id} was not found.");
            }

            //var receiptId = (long)id;
            var lines = await _employee.Where(p => p.ProductionTeamId == id, ct);

            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _employee.DeleteRange(lines, token);
                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<ProductionTeamDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<ProductionTeamDto>>(await _repo.GetAll(ct));
        
        public async Task<ProductionTeamDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<ProductionTeamDto>(entity);
        }

        public async Task<Result<ProductionTeamDto>> UpdateAsync(ulong id, ProductionTeamRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<ProductionTeamDto>.Failure($"Production team {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.TeamCode == request.TeamCode, ct);
            if (existing is not null)
            {
                return Result<ProductionTeamDto>.Failure($"Production team '{request.TeamCode}' already exists.");
            }

            //var receiptId = (long)id;
            var stored = await _employee.Where(l => l.ProductionTeamId == id, ct);
            var lines = request.Employees;
            var keptIds = (lines ?? new List<ProductionTeamEmployeeRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreign = keptIds.Where(lineId => stored.All(s => s.Id != lineId)).ToList();
            if (foreign.Count > 0)
            {
                return Result<ProductionTeamDto>.Failure($"Line(s) {string.Join(", ", foreign)} do not belong to Production team {id}.");
            }

            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (lines is not null)
                {
                    await _employee.DeleteRange(stored.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in lines.Where(l => l.Id != 0))
                    {
                        var target = stored.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _employee.Update(target, token);
                    }

                    await _employee.AddRange(lines.Where(l => l.Id == 0).Select(line => ToLineEntity(line, id)).ToList(), token);
                }

                return Result<ProductionTeamDto>.Success(_mapper.Map<ProductionTeamDto>(entity));
            }, ct);
        }

        private Entities.ProductionTeamEmployee ToLineEntity(ProductionTeamEmployeeRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.ProductionTeamEmployee>(line);
            entity.ProductionTeamId = id;
            return entity;
        }
    }


    public class ProductionTeamEmployeeService : IProductionTeamEmployeeService
    {
        private readonly IRepository<Entities.ProductionTeamEmployee> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductionTeamEmployeeService(
            IRepository<Entities.ProductionTeamEmployee> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductionTeamEmployeeDto>> CreateAsync(ProductionTeamEmployeeRequest request, CancellationToken ct = default)
        {
            //var existing = await _repository.FirstOrDefault(p => p.EmployeeCode == request.EmployeeCode, ct);
            //if (existing is not null)
            //{
            //    return Result<ProductionTeamEmployeeDto>.Failure($"Employee code '{request.EmployeeCode}' already exists.");
            //}

            var entity = _mapper.Map<Entities.ProductionTeamEmployee>(request);
            await _repository.Add(entity, ct);
            return Result<ProductionTeamEmployeeDto>.Success(_mapper.Map<ProductionTeamEmployeeDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Production team employee {id} was not found.");
        }

        public async Task<List<ProductionTeamEmployeeDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<ProductionTeamEmployeeDto>>(await _repository.GetAll(ct));
        public async Task<ProductionTeamEmployeeDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<ProductionTeamEmployeeDto>(entity);
        }

        public async Task<Result<ProductionTeamEmployeeDto>> UpdateAsync(ulong id, ProductionTeamEmployeeRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<ProductionTeamEmployeeDto>.Failure($"Production team employee {id} was not found.");
            }

            //var existing = await _repository.FirstOrDefault(p => p.Id != id && p.EmployeeCode == request.EmployeeCode, ct);
            //if (existing is not null)
            //{
            //    return Result<ProductionTeamEmployeeDto>.Failure($"Employee code '{request.EmployeeCode}' already exists.");
            //}

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<ProductionTeamEmployeeDto>.Success(_mapper.Map<ProductionTeamEmployeeDto>(entity));
        }
    }
}
