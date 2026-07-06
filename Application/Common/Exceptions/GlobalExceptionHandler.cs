using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var problemDetails = exception switch
        {
            ApiException e => new ProblemDetails
            {
                Status = e.StatusCode,
                Title = e.Message,
                Extensions = { { "errors", e.Errors } }
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected error occured"
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
