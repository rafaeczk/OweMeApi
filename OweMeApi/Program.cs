using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using Scalar.AspNetCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OweMeApi.Services;

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
            ValidateIssuer = false, // in production set to true
            ValidateAudience = false // in production set to true
        };
    });

builder.Services.AddScoped<AuthService>();

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
