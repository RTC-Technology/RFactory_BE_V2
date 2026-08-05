namespace RFactory.Application.Modules.Administration.DTOs;

/// <summary>
/// Read model returned to API clients for a user. Never exposes PasswordHash or RefreshToken.
/// </summary>
public class UserDto
{
    public ulong Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string LoginName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public long? OrganizationId { get; set; }
}

/// <summary>
/// Payload for creating a user. <see cref="Password"/> is hashed before being persisted.
/// </summary>
public class CreateUserRequest
{
    public string Code { get; set; } = string.Empty;
    public string LoginName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public long? OrganizationId { get; set; }
}

/// <summary>
/// Payload for updating a user. <see cref="Password"/> is optional; when null the
/// existing password hash is left untouched.
/// </summary>
public class UpdateUserRequest
{
    public string Code { get; set; } = string.Empty;
    public string LoginName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public long? OrganizationId { get; set; }
}
