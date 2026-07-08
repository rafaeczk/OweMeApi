using Application.Common.Interfaces;
using Infrastructure.Identity;

namespace WebAPI;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
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
                      .AllowAnyMethod();
            });
        });
    }
}
