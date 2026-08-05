using RFactory.Application.Modules.Administration.DTOs;
using RFactory.Shared.Results;

namespace RFactory.Application.Modules.Administration.Services;

/// <summary>
/// Application service for user accounts. Controllers depend on this interface rather than
/// the repository directly, keeping password hashing and uniqueness rules out of the API layer.
/// </summary>
public interface IUserService
{
    Task<List<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDto?> GetByIdAsync(ulong id, CancellationToken ct = default);
    Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<UserDto>> UpdateAsync(ulong id, UpdateUserRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(ulong id, CancellationToken ct = default);
}
