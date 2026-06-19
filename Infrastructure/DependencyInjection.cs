using Infrastructure.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("No connection string found");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();
    }
}
