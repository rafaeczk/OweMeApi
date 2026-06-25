using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Common.Exceptions;

namespace Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => (object?)g.Select(f => f.ErrorMessage).ToArray()
                );

            throw new ApiException("Validation failed", StatusCodes.Status400BadRequest)
            {
                Errors = errors
            };
        }

        return await next(ct);
    }
}
