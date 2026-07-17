using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(e => new ErrorItem(
                            CleanMessage(e.Value!.Errors.First().ErrorMessage),
                            CleanPropertyPath(e.Key)
                        ))
                        .ToList();

                    var response = new ErrorResponse
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                    origin.StartsWith("http://localhost:") ||
                    origin == "https://owe-me.vercel.app")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }

    private static string CleanPropertyPath(string key)
    {
        return key.Replace("$.", "").Replace("['", "").Replace("']", "");
    }

    private static string CleanMessage(string message)
    {
        if (message.Contains("Path: "))
        {
            return "Invalid format for this field.";
        }
        return message;
    }
}
