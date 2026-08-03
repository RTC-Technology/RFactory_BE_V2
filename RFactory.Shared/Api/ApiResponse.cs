using System.Net;

namespace RFactory.Shared.Api;

/// <summary>
/// Uniform response envelope returned by every API endpoint.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }
}

/// <summary>
/// Helper factory to build <see cref="ApiResponse{T}"/> instances with consistent status codes.
/// </summary>
public static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(T data, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            Success = true,
            StatusCode = (int)statusCode,
            Message = message,
            Data = data
        };

    public static ApiResponse<object?> Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, object? errors = null)
        => new()
        {
            Success = false,
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors
        };
}
