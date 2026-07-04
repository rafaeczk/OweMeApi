using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Application.Common;

public enum ErrorCode { None, NotFound, BadRequest, Conflict, Unauthorized, InternalError }

public class HandlerResult
{
    public bool IsSuccess { get; }
    public IEnumerable<string> Errors { get; }
    public ErrorCode ErrorCode { get; }

    protected HandlerResult(bool isSuccess, IEnumerable<string> errors, ErrorCode errorCode)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        ErrorCode = errorCode;
    }

    public static implicit operator HandlerResult(HandlerFailureResult failure)
    {
        return new(false, failure.Errors, failure.ErrorCode);
    }

    public static HandlerResult Success() => new(true, [], ErrorCode.None);
    public static HandlerFailureResult Failure(string error, ErrorCode errorCode) => new([error], errorCode);
    public static HandlerFailureResult Failure(IEnumerable<string> errors, ErrorCode errorCode) => new(errors, errorCode);

    protected ActionResult CreateActionResult(object? value)
    {
        if (IsSuccess) return value != null ? new OkObjectResult(value) : new OkResult();

        var problemDetails = new ProblemDetails
        {
            Status = ErrorCode switch
            {
                ErrorCode.NotFound => StatusCodes.Status404NotFound,
                ErrorCode.BadRequest => StatusCodes.Status400BadRequest,
                ErrorCode.Conflict => StatusCodes.Status409Conflict,
                ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            },
            Extensions = { { "errors", new Dictionary<string, object>() { { "general", Errors } } } }
        };

        return ErrorCode switch
        {
            ErrorCode.NotFound => new NotFoundObjectResult(problemDetails),
            ErrorCode.Conflict => new ConflictObjectResult(problemDetails),
            ErrorCode.Unauthorized => new UnauthorizedObjectResult(problemDetails),
            _ => new BadRequestObjectResult(problemDetails)
        };
    }

    public virtual ActionResult ToActionResult() => CreateActionResult(null);
}

public class HandlerResult<T> : HandlerResult
{
    public T? Value { get; }

    private HandlerResult(T value) : base(true, [], ErrorCode.None) => Value = value;
    private HandlerResult(IEnumerable<string> errors, ErrorCode errorCode) : base(false, errors, errorCode) => Value = default;

    public static implicit operator HandlerResult<T>(HandlerFailureResult failure)
    {
        return new(failure.Errors, failure.ErrorCode);
    }

    public static implicit operator HandlerResult<T>(T value)
    {
        return new(value);
    }

    public static HandlerResult<T> Success(T value) => new(value);

    public override ActionResult ToActionResult() => CreateActionResult(Value);
}

public class HandlerFailureResult(IEnumerable<string> errors, ErrorCode errorCode)
{
    public IEnumerable<string> Errors { get; } = errors;
    public ErrorCode ErrorCode { get; } = errorCode;
}
