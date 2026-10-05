using AutoMapper;
using RFactory.Application.Modules.MasterData.DTOs;
using RFactory.Application.Modules.Quality.DTOs;
using RFactory.Application.Modules.Quality.Services;
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
    public class CustomerService:ICustomerService
    {
        private readonly IRepository<Entities.Customer> _repo;
        private readonly IRepository<Entities.CustomerContact> _contactRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(
            IRepository<Entities.Customer> repo,
            IRepository<Entities.CustomerContact> contactRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repo = repo;
            _contactRepo = contactRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CustomerDto>> CreateAsync(CustomerRequest request, CancellationToken ct = default)
        {
            var existing = await _repo.FirstOrDefault(t => t.CustomerCode == request.CustomerCode, ct);
            if (existing is not null)
            {
                return Result<CustomerDto>.Failure($"Customer '{request.CustomerCode}' already exists.");
            }

            var entity = _mapper.Map<Entities.Customer>(request);
            var contacts = request.CustomerContacts ?? new List<CustomerContactRequest>();
            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Add(entity, token);

                // 2. Map + insert Details
                await _contactRepo.AddRange(contacts.Select(contact => ToContactEntity(contact, entity.Id)).ToList(), token);

                return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(entity));
            }, ct);
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result.Failure($"Customer {id} was not found.");
            }

            //var receiptId = (long)id;
            var items = await _contactRepo.Where(p => p.CustomerId == id, ct);
            // The lines belong to this receipt and nothing else, so they go with it instead of
            // blocking the delete — deleting is soft on both, and the pair moves together.
            return await _unitOfWork.ExecuteAsync<Result>(async token =>
            {
                await _contactRepo.DeleteRange(items, token);

                await _repo.Delete(entity, token);
                return Result.Success();
            }, ct);
        }

        public async Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<List<CustomerDto>>(await _repo.GetAll(ct));

        public async Task<CustomerDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            return entity is null ? null : _mapper.Map<CustomerDto>(entity);
        }

        public async Task<Result<CustomerDto>> UpdateAsync(ulong id, CustomerRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetById(id, ct);
            if (entity is null)
            {
                return Result<CustomerDto>.Failure($"Customer {id} was not found.");
            }

            var existing = await _repo.FirstOrDefault(t => t.Id != id && t.CustomerCode == request.CustomerCode, ct);
            if (existing is not null)
            {
                return Result<CustomerDto>.Failure($"Customer '{request.CustomerCode}' already exists.");
            }

            //var receiptId = (long)id;
            var storedItems = await _contactRepo.Where(l => l.CustomerId == id, ct);
            var contacts = request.CustomerContacts;
            var keptIds = (contacts ?? new List<CustomerContactRequest>())
                .Where(line => line.Id != 0)
                .Select(line => line.Id)
                .ToHashSet();

            // The list replaces the whole set, so an id from another receipt would be edited
            // here and dropped from where it belongs. Reject the payload instead.
            var foreigns = keptIds.Where(lineId => storedItems.All(s => s.Id != lineId)).ToList();
            if (foreigns.Count > 0)
            {
                return Result<CustomerDto>.Failure($"Contact(s) {string.Join(", ", foreigns)} do not belong to Customer {id}.");
            }


            _mapper.Map(request, entity);

            return await _unitOfWork.ExecuteAsync(async token =>
            {
                await _repo.Update(entity, token);

                // A null list means the caller is editing the header only; an empty one means
                // the receipt really has no lines left.
                if (contacts is not null)
                {
                    await _contactRepo.DeleteRange(storedItems.Where(s => !keptIds.Contains(s.Id)).ToList(), token);

                    foreach (var line in contacts.Where(l => l.Id != 0))
                    {
                        var target = storedItems.First(s => s.Id == line.Id);
                        _mapper.Map(line, target);
                        await _contactRepo.Update(target, token);
                    }

                    await _contactRepo.AddRange(contacts.Where(l => l.Id == 0).Select(line => ToContactEntity(line, id)).ToList(), token);
                }

                return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(entity));
            }, ct);
        }

        private Entities.CustomerContact ToContactEntity(CustomerContactRequest line, ulong id)
        {
            var entity = _mapper.Map<Entities.CustomerContact>(line);
            entity.CustomerId = id;
            return entity;
        }
    }

    public class CustomerContactService : ICustomerContactService
    {
        private readonly IRepository<Entities.CustomerContact> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerContactService(
            IRepository<Entities.CustomerContact> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CustomerContactDto>> CreateAsync(CustomerContactRequest request, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerContact>(request);
            await _repository.Add(entity, ct);
            return Result<CustomerContactDto>.Success(_mapper.Map<CustomerContactDto>(entity));
        }

        public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
        {
            var deleted = await _repository.DeleteById(id, ct);
            return deleted ? Result.Success() : Result.Failure($"Customer contact {id} was not found.");
        }

        public async Task<List<CustomerContactDto>> GetAllAsync(CancellationToken ct = default)
         => _mapper.Map<List<CustomerContactDto>>(await _repository.GetAll(ct));
        public async Task<CustomerContactDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            return entity is null ? null : _mapper.Map<CustomerContactDto>(entity);
        }

        public async Task<Result<CustomerContactDto>> UpdateAsync(ulong id, CustomerContactRequest request, CancellationToken ct = default)
        {
            var entity = await _repository.GetById(id, ct);
            if (entity is null)
            {
                return Result<CustomerContactDto>.Failure($"Customer contact {id} was not found.");
            }

            _mapper.Map(request, entity);
            await _repository.Update(entity, ct);
            return Result<CustomerContactDto>.Success(_mapper.Map<CustomerContactDto>(entity));
        }
    }
}
