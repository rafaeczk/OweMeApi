using Application.Common.Interfaces;
using Infrastructure.Identity;

namespace WebAPI;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddHttpContextAccessor();
    }
}
