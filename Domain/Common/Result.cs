namespace Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public IEnumerable<string> Errors { get; }

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
    public static FailureResult Failure(string error, FailureReason reason = FailureReason.Failure) => new([error], reason);
    public static FailureResult Failure(IEnumerable<string> errors, FailureReason reason = FailureReason.Failure) => new(errors, reason);
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

public enum FailureReason { None, NotFound, Unauthorized, Validation, Conflict, Failure }

public class FailureResult(IEnumerable<string> errors, FailureReason reason)
{
    public IEnumerable<string> Errors { get; } = errors;
    public FailureReason Reason { get; } = reason;
}
