using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Middleware;

public sealed class GlobalExceptionMiddleware
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failure.");
            await WriteProblemDetails(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var payload = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
