using System.Net;
using Microsoft.AspNetCore.Mvc;
using RFactory.Shared.Api;
using RFactory.Shared.Exceptions;

namespace RFactory.API.Middleware;

/// <summary>
/// Catches unhandled exceptions and maps them to consistent <see cref="ApiResponse{T}"/> envelopes.
/// Eliminates per-action try/catch blocks in controllers.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteError(context, ex.Message, HttpStatusCode.NotFound);
        }
        catch (BusinessException ex)
        {
            await WriteError(context, ex.Message, (HttpStatusCode)ex.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteError(context, "An unexpected error occurred.", HttpStatusCode.InternalServerError);
        }
    }

    private static async Task WriteError(HttpContext context, string message, HttpStatusCode statusCode)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var response = ApiResponseFactory.Fail(message, statusCode);
        await context.Response.WriteAsJsonAsync(response);
    }
}
