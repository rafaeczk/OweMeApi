using Microsoft.AspNetCore.Mvc;

namespace OweMeApi.Common;

public enum ErrorCode { None, NotFound, BadRequest, Conflict, Unauthorized }

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ErrorCode ErrorCode { get; }

    protected Result(bool isSuccess, string? error, ErrorCode errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static implicit operator Result(FailureResult failure)
    {
        return new(false, failure.Error, failure.ErrorCode);
    }

    public static Result Success() => new(true, null, ErrorCode.None);
    public static FailureResult Failure(string error, ErrorCode errorCode) => new(error, errorCode);

    protected ActionResult CreateActionResult(object? value)
    {
        if (IsSuccess) return value != null ? new OkObjectResult(value) : new OkResult();

        return ErrorCode switch
        {
            ErrorCode.NotFound => new NotFoundObjectResult(Error),
            ErrorCode.BadRequest => new BadRequestObjectResult(Error),
            ErrorCode.Conflict => new ConflictObjectResult(Error),
            ErrorCode.Unauthorized => new UnauthorizedObjectResult(Error),
            _ => new BadRequestObjectResult(Error)
        };
    }

    public virtual ActionResult ToActionResult() => CreateActionResult(null);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null, ErrorCode.None) => Value = value;
    private Result(string error, ErrorCode errorCode) : base(false, error, errorCode) => Value = default;

    public static implicit operator Result<T>(FailureResult failure)
    {
        return new(failure.Error, failure.ErrorCode);
    }

    public static implicit operator Result<T>(T value)
    {
        return new(value);
    }

    public static Result<T> Success(T value) => new(value);

    public override ActionResult ToActionResult() => CreateActionResult(Value);
}

public class FailureResult(string error, ErrorCode errorCode)
{
    public string Error { get; } = error;
    public ErrorCode ErrorCode { get; } = errorCode;
}
