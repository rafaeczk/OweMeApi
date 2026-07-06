using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

internal class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WebAPI"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<AppDbContextFactory>()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if(string.IsNullOrEmpty(connectionString))
            throw new Exception("Connection string 'DefaultConnection' not found!");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}