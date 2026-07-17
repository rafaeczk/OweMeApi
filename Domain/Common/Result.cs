namespace Domain.Common;

public enum FailureReason { None, NotFound, BadRequest, Conflict, Unauthorized, InternalError }

public class Result
{
    public bool IsSuccess { get; }
    public IEnumerable<string> Errors { get; }
    public FailureReason FailureReason { get; }

    protected Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static implicit operator Result(FailureResult failure)
    {
        return new(false, failure.Errors);
    }

    public static Result Success() => new(true, []);
    public static FailureResult Failure(string error, FailureReason reason) => new([error], reason);
    public static FailureResult Failure(IEnumerable<string> errors, FailureReason reason) => new(errors, reason);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, []) => Value = value;
    private Result(IEnumerable<string> errors) : base(false, errors) => Value = default;

    public static implicit operator Result<T>(FailureResult failure)
    {
        return new(failure.Errors);
    }

    public static implicit operator Result<T>(T value)
    {
        return new(value);
    }

    public static Result<T> Success(T value) => new(value);
}

public class FailureResult(IEnumerable<string> errors, FailureReason reason)
{
    public IEnumerable<string> Errors { get; } = errors;
    public FailureReason FailureReason { get; } = reason;
}
