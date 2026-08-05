namespace RFactory.Shared.Abstractions;

/// <summary>
/// Abstraction of the current authenticated user, resolved per request in the API layer.
/// Lives in Shared so Infrastructure (interceptors) can consume it without referencing API/Application.
/// </summary>
public interface IUser
{
    string? Id { get; }
    string? UserName { get; }
    bool IsAdmin { get; }
}
