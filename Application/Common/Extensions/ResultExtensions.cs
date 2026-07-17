using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Common;
using Application.Common.Exceptions;

namespace Application.Common.Extensions;

public static class ResultExtensions
{
    private static ActionResult CreateActionResult(Result result, object? value)
    {
        if (result.IsSuccess) return value != null ? new OkObjectResult(value) : new OkResult();

        var problemDetails = new ErrorResponse()
        {
            Status = result.FailureReason switch
            {
                FailureReason.NotFound => StatusCodes.Status404NotFound,
                FailureReason.BadRequest => StatusCodes.Status400BadRequest,
                FailureReason.Conflict => StatusCodes.Status409Conflict,
                FailureReason.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            },
            Errors = result.Errors.Select(message => new ErrorItem(message, null))
        };

        return result.FailureReason switch
        {
            FailureReason.NotFound => new NotFoundObjectResult(problemDetails),
            FailureReason.Conflict => new ConflictObjectResult(problemDetails),
            FailureReason.Unauthorized => new UnauthorizedObjectResult(problemDetails),
            _ => new BadRequestObjectResult(problemDetails)
        };
    }

    public static ActionResult ToActionResult(this Result result) => CreateActionResult(result, null);

    public static ActionResult ToActionResult<T>(this Result<T> result) => CreateActionResult(result, result.Value);

    public static ActionResult ToActionResult<T>(this FailureResult result) => CreateActionResult(result, null);
}
