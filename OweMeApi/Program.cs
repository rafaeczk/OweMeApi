using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OweMeApi.Behaviours;
using OweMeApi.Common;
using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data;
using OweMeApi.Extensions;
using OweMeApi.Filters;
using OweMeApi.Modules.Auth;
using OweMeApi.Modules.Debts;
using OweMeApi.Modules.FriendCodes;
using OweMeApi.Modules.Friends;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CONFIG

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
});

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

// SERVICES

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FriendCodesService>();
builder.Services.AddScoped<FriendsService>();
builder.Services.AddScoped<DebtsService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCustomAuthorization();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// BUILD

var app = builder.Build();

// MOCK IF DEV

if (!app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    await Mock.Init(scope.ServiceProvider, app.Environment);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapScalarApiReference();

app.Run();
