using Microsoft.Extensions.DependencyInjection;
using Domain.Enums;

namespace Infrastructure.Identity;

public static class AuthPolicies
{
    public const string Admin = nameof(Admin);
    public const string AdminOrUser = nameof(AdminOrUser);

    public static IServiceCollection AddAuthorizationBuilderWithPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Admin, policy => policy.RequireRole(UserRole.Admin))
            .AddPolicy(AdminOrUser, policy => policy.RequireRole(UserRole.User, UserRole.Admin));

        return services;
    }
}
