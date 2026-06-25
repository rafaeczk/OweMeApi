using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Application.Common.Exceptions;

namespace Application.Common;

public enum ErrorCode { None, NotFound, BadRequest, Conflict, Unauthorized, InternalError }

public class HandlerResult
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ErrorCode ErrorCode { get; }

    protected HandlerResult(bool isSuccess, string? error, ErrorCode errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static implicit operator HandlerResult(HandlerFailureResult failure)
    {
        return new(false, failure.Error, failure.ErrorCode);
    }

    public static HandlerResult Success() => new(true, null, ErrorCode.None);
    public static HandlerFailureResult Failure(string error, ErrorCode errorCode) => new(error, errorCode);

    protected ActionResult CreateActionResult(object? value)
    {
        if (IsSuccess) return value != null ? new OkObjectResult(value) : new OkResult();

        throw new ApiException(
            Error ?? "Unexpected error occured",
            ErrorCode switch
            {
                ErrorCode.NotFound => StatusCodes.Status404NotFound,
                ErrorCode.BadRequest => StatusCodes.Status400BadRequest,
                ErrorCode.Conflict => StatusCodes.Status409Conflict,
                ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            });
    }

    public virtual ActionResult ToActionResult() => CreateActionResult(null);
}

public class HandlerResult<T> : HandlerResult
{
    public T? Value { get; }

    private HandlerResult(T value) : base(true, null, ErrorCode.None) => Value = value;
    private HandlerResult(string error, ErrorCode errorCode) : base(false, error, errorCode) => Value = default;

    public static implicit operator HandlerResult<T>(HandlerFailureResult failure)
    {
        return new(failure.Error, failure.ErrorCode);
    }

    public static implicit operator HandlerResult<T>(T value)
    {
        return new(value);
    }

    public static HandlerResult<T> Success(T value) => new(value);

    public override ActionResult ToActionResult() => CreateActionResult(Value);
}

public class HandlerFailureResult(string error, ErrorCode errorCode)
{
    public string Error { get; } = error;
    public ErrorCode ErrorCode { get; } = errorCode;
}
