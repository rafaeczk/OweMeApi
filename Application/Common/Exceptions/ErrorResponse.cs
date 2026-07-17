using Microsoft.AspNetCore.Mvc;

namespace Application.Common.Exceptions;

public class ErrorResponse
{
    public int Status { get; set; }
    public IEnumerable<ErrorItem> Errors { get; set; } = [];

    public static implicit operator ProblemDetails(ErrorResponse errorResponse)
    {
        return new ProblemDetails
        {
            Status = errorResponse.Status,
            Extensions =
            {
                { "Errors", errorResponse.Errors },
            }
        };
    }
}
