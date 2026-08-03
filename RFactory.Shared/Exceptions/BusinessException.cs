namespace RFactory.Shared.Exceptions;

/// <summary>
/// Thrown for expected business rule violations. The global exception middleware
/// maps this to an HTTP 400/409 response with the message surfaced to the client.
/// </summary>
public class BusinessException : Exception
{
    public int StatusCode { get; }

    public BusinessException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

/// <summary>
/// Thrown when a requested entity does not exist. Mapped to HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}
