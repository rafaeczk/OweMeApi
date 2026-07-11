using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // IDENTITY

        builder.Services
            .AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwtSecret = builder.Configuration["JwtSettings:Secret"]
            ?? throw new Exception("'JwtSettings:Secret' not found in configuration!");

        var key = Encoding.ASCII.GetBytes(jwtSecret);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? throw new Exception("'JwtSettings:Issuer' not found in configuration!"),
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? throw new Exception("'JwtSettings:Audience' not found in configuration!"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.TryGetValue("auth_token", out var token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddAuthorizationBuilderWithPolicies();

        // DATABASE

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

        // OTHER

        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
    }
}