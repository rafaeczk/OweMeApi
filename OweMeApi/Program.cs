using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OweMeApi.Data;
using OweMeApi.Extensions;
using OweMeApi.Modules.Auth;
using OweMeApi.Modules.FriendCodes;
using OweMeApi.Modules.Friends;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// SERVICES

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            // IssuerSigningKeyResolver is invoked during token validation, so it can read the current configuration
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                var key = builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt Key not found in the configuration");
                return [new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))];
            },
            ValidateIssuer = false, // DO ZMIANY
            ValidateAudience = false // DO ZMIANY
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[AuthService.CookieName];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FriendCodesService>();
builder.Services.AddScoped<FriendsService>();

builder.Services.AddCustomAuthorization();

// BUILD

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapScalarApiReference();

app.Run();
