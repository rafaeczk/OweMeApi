using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthorizationBuilderWithPolicies();

        builder.Services.AddScoped<AuditableEntityInterceptor>();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Connection string 'DefaultConnection' not found!");

        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var auditableEntityInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            options.UseNpgsql(connectionString)
                .AddInterceptors(auditableEntityInterceptor)
                .LogTo(Console.WriteLine, LogLevel.Information);
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.None
                : CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
    }
}