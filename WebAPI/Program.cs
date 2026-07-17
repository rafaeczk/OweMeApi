using Application;
using Infrastructure;
using Infrastructure.Data;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.AddWebServices();
builder.AddApplicationServices();
builder.AddInfrastructureServices();

builder.Services.AddCors();

var app = builder.Build();

app.UseExceptionHandler();

await app.InitialiseDatabase(app.Environment.IsDevelopment());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
