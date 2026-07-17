using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Application.Common.Validation;

namespace Application.Common.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        ProblemDetails problemDetails = exception switch
        {
            System.Text.Json.JsonException e => new ErrorResponse
            {
                Status = StatusCodes.Status400BadRequest,
                Errors = [new ErrorItem("Invalid data format", e.Path)]
            },
            ValidationException e => new ErrorResponse
            {
                Status = StatusCodes.Status400BadRequest,
                Errors = e.Errors
            },
            _ => new ErrorResponse
            {
                Status = StatusCodes.Status500InternalServerError,
                Errors = [new ErrorItem("Unexpected error occured", null)]
            }
        };

        var traceId = httpContext.TraceIdentifier;

        problemDetails.Extensions.Add("traceId", traceId);

        logger.LogError(exception, "Error: {Message}, TraceId: {TraceId}", problemDetails.Title, traceId);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }
}
