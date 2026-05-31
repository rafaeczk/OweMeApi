namespace OweMeApi.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole("ADMIN"))
            .AddPolicy("AdminOrModerator", policy => policy.RequireRole("ADMIN", "MODERATOR"))
            .AddPolicy("User", policy => policy.RequireRole("USER"))
            .AddPolicy("All", policy => policy.RequireRole("USER", "ADMIN", "MODERATOR"));

        return services;
    }
}

