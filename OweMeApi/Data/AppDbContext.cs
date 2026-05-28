using Microsoft.EntityFrameworkCore;
using OweMeApi.Data.Entities;

namespace OweMeApi.Data
{
    public class AppDbContext : DbContext
    {
        // Konstruktor przekazujący opcje (np. connection string) do bazy
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("gen_random_uuid()");
        }

        // Deklaracja tabel (DbSet)
        // Każda klasa modelu, którą chcesz mieć w bazie, musi być tutaj
        public DbSet<User> Users { get; set; }

        // Jeśli będziesz miał inne tabele, dodajesz je analogicznie:
        // public DbSet<Product> Products { get; set; }
    }
}