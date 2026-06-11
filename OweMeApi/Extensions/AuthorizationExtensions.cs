using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole(SystemUserRole.Admin))
            .AddPolicy("AdminOrModerator", policy => policy.RequireRole(SystemUserRole.Admin, SystemUserRole.Moderator))
            .AddPolicy("User", policy => policy.RequireRole(SystemUserRole.User))
            .AddPolicy("All", policy => policy.RequireRole(SystemUserRole.User, SystemUserRole.Admin, SystemUserRole.Moderator));

        return services;
    }
}

