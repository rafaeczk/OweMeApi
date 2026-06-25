using Application;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Scalar.AspNetCore;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.AddWebServices();
builder.AddApplicationServices();
builder.AddInfrastructureServices();

builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddCors();

var app = builder.Build();

await app.InitialiseDatabase(app.Environment.IsDevelopment());

app.UseHttpsRedirection();
app.UseCors(static builder =>
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.MapOpenApi();
app.MapScalarApiReference();

app.Map("/", () => Results.Redirect("/scalar"));

app.MapIdentityApi<AppUser>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
