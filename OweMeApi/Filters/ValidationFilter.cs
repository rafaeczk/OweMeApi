using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using OweMeApi.Common;

namespace OweMeApi.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach(var arg in context.ActionArguments.Values)
        {
            if (arg == null)
                continue;

            if (arg.GetType().Namespace == "System") 
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());

            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(arg);

                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    throw new ApiException("Validation failed", StatusCodes.Status400BadRequest)
                    {
                        Errors = result.Errors.GroupBy(e => e.PropertyName)
                              .ToDictionary(g => g.Key, g => (object?)g.Select(e => e.ErrorMessage).ToArray())
                    };
                }
            }
        }

        await next();
    }
}
