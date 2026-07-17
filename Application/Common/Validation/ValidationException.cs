using Application.Common.Exceptions;

namespace Application.Common.Validation;

public class ValidationException(IEnumerable<ErrorItem> errors) : Exception
{
    public IEnumerable<ErrorItem> Errors { get; set; } = errors;
}
