using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OweMeApi.Common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Unexpected error occured: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unexpected error occured.",
            Extensions = new Dictionary<string, object?> { }
        };

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }
}
