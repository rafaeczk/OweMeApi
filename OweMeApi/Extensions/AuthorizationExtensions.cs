using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole(UserRole.Admin))
            .AddPolicy("AdminOrModerator", policy => policy.RequireRole(UserRole.Admin, UserRole.Moderator))
            .AddPolicy("User", policy => policy.RequireRole(UserRole.User))
            .AddPolicy("All", policy => policy.RequireRole(UserRole.User, UserRole.Admin, UserRole.Moderator));

        return services;
    }
}
