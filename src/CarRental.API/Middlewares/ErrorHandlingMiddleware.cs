/// MIT License © 2025 Martín Duhalde + ChatGPT

using CarRental.Domain.Exceptions;
using CarRental.UseCases.Common.Dtos;

using System.Net;
using System.Text.Json;

using FluentValidationException = FluentValidation.ValidationException;

namespace CarRental.API.Middlewares;


public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate                    /**/ _next;
    private readonly ILogger<ErrorHandlingMiddleware>   /**/ _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   /**/ = next;
        _logger /**/ = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode  /**/ status;
        string          /**/ errorMessage;
        string?         /**/ details = null;


        if (exception is FluentValidationException validationEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            // Devolvemos el diccionario de errores directamente como JSON
            return context.Response.WriteAsJsonAsync(errors);
        }

        switch (exception)
        {
            case InvalidCredentialsException:
                status                          /**/ = HttpStatusCode.Unauthorized;
                errorMessage                    /**/ = "🔐 Invalid credentials";
                break;

            case ConcurrencyConflictException:
                status                          /**/ = HttpStatusCode.Conflict;
                errorMessage                    /**/ = "🔄 Concurrency conflict: the entity was modified by another user or process.";
                break;
           
            case DomainNotFoundException:
                status                          /**/ = HttpStatusCode.NotFound;
                errorMessage                    /**/ = "🚫 Resource not available: the entity was not found or has been deleted.";
                break;

            case DomainException:
                status                          /**/ = HttpStatusCode.BadRequest;
                errorMessage                    /**/ = "📘 Business rule violation (Domain error)";
                break;

            case UnauthorizedAccessException:
                status                          /**/ = HttpStatusCode.Unauthorized;
                errorMessage                    /**/ = "🔒 Unauthorized access";
                break;

            case ArgumentException:
                status                          /**/ = HttpStatusCode.InternalServerError;
                errorMessage                    /**/ = "💥 Internal server error";
                break;

            default:
                status                          /**/ = HttpStatusCode.InternalServerError;
                errorMessage                    /**/ = "💥 Internal Server Error";
                break;
        }

        /// Solo mostrar detalles en desarrollo:
        if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
            details = exception.Message;

        context.Response.StatusCode = (int)status;

        var errorResponse = new ErrorResponseDto
        {
            Error   /**/ = errorMessage,
            Details /**/ = details
        };

        var json = JsonSerializer.Serialize(errorResponse);

        return context.Response.WriteAsync(json);
    }
}
