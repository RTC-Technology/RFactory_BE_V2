using AutoMapper;
using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Infrastructure.Entities;
using RFactory.Infrastructure.Persistence;
using RFactory.Shared.Results;
using RFactory.Shared.Security;

namespace RFactory.Application.Modules.Administration.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IRepository<User> repository, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _repository.GetAll(ct);
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(ulong id, CancellationToken ct = default)
    {
        var user = await _repository.GetById(id, ct);
        return user is null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var existing = await _repository.FirstOrDefault(
            u => u.LoginName == request.LoginName || u.Code == request.Code, ct);
        if (existing is not null)
        {
            return Result<UserDto>.Failure($"Login name '{request.LoginName}' or code '{request.Code}' already exists.");
        }

        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordHasher.Hash(request.Password);
        await _repository.Add(user, ct);
        return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
    }

    public async Task<Result<UserDto>> UpdateAsync(ulong id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _repository.GetById(id, ct);
        if (user is null)
        {
            return Result<UserDto>.Failure($"User {id} was not found.");
        }

        var existing = await _repository.FirstOrDefault(
            u => u.Id != id && (u.LoginName == request.LoginName || u.Code == request.Code), ct);
        if (existing is not null)
        {
            return Result<UserDto>.Failure($"Login name '{request.LoginName}' or code '{request.Code}' already exists.");
        }

        _mapper.Map(request, user);
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _passwordHasher.Hash(request.Password);
        }

        await _repository.Update(user, ct);
        return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
    }

    public async Task<Result> DeleteAsync(ulong id, CancellationToken ct = default)
    {
        var deleted = await _repository.DeleteById(id, ct);
        return deleted ? Result.Success() : Result.Failure($"User {id} was not found.");
    }
}
