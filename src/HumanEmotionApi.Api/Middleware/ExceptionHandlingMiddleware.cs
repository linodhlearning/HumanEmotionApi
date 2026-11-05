using System.Net;
using System.Text.Json;
using HumanEmotionApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HumanEmotionApi.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred during request execution: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                notFound.Message
            ),
            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                "Domain Validation Error",
                domainEx.Message
            ),
            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                "Invalid Argument",
                argEx.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred while processing your request."
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}

