using System.Diagnostics;
using System.Net;
using System.Text.Json;
using CoreBankingSystem.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreBankingSystem.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected or request aborted; don't try to write a response
            _logger.LogInformation("Request aborted by client. TraceId: {TraceId}", Activity.Current?.Id ?? context.TraceIdentifier);
            // No response writing; simply return to stop the pipeline
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var statusCode = GetStatusCode(exception);

        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
        {
            // If we reach here (e.g., non-standard cancellation), avoid writing a body.
            logger.LogInformation("Operation canceled. TraceId: {TraceId}", traceId);
            return;
        }

        logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

        var problem = new ProblemDetails
        {
            Title = GetTitleForStatusCode(statusCode),
            Detail = exception.Message,
            Status = statusCode,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = traceId;
        problem.Extensions["source"] = exception.Source;

        // If the response has already started, we can't modify headers/body
        if (context.Response.HasStarted)
        {
            logger.LogWarning("The response has already started, the error handling middleware will not write a response. TraceId: {TraceId}", traceId);
            return;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        try
        {
            await context.Response.WriteAsync(json);
        }
        catch (OperationCanceledException)
        {
            // Swallow write cancellation (client disconnected during error write)
            logger.LogInformation("Response write canceled. TraceId: {TraceId}", traceId);
        }
        catch (ObjectDisposedException)
        {
            // Ignore if response body is disposed due to connection abort
            logger.LogInformation("Response object disposed before writing error. TraceId: {TraceId}", traceId);
        }
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        BadRequestException => (int)HttpStatusCode.BadRequest,
        NotFoundException => (int)HttpStatusCode.NotFound,
        ArgumentException => (int)HttpStatusCode.BadRequest,
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,
        DbUpdateException => (int)HttpStatusCode.Conflict,
        OperationCanceledException => StatusCodes.Status499ClientClosedRequest, // Non-standard but commonly used
        _ => (int)HttpStatusCode.InternalServerError
    };

    private static string GetTitleForStatusCode(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        499 => "Client Closed Request",
        _ => "An unexpected error occurred"
    };
}
