using System.Net;
using System.Text.Json;
using HumanEmotionApi.Api.Middleware;
using HumanEmotionApi.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HumanEmotionApi.Tests.Api;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        bool nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_ShouldReturn404ProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (_) => throw new NotFoundException("Item not found");
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
        Assert.Equal("Resource Not Found", problemDetails.Title);
        Assert.Equal("Item not found", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WhenDomainException_ShouldReturn400ProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (_) => throw new DomainException("Validation failed");
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Domain Validation Error", problemDetails.Title);
        Assert.Equal("Validation failed", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WhenArgumentException_ShouldReturn400ProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (_) => throw new ArgumentException("Invalid argument specified");
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Invalid Argument", problemDetails.Title);
        Assert.Equal("Invalid argument specified", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_ShouldReturn500ProblemDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (_) => throw new InvalidOperationException("Fatal crash");
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(problemDetails);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Contains("unexpected error", problemDetails.Detail);
    }
}

