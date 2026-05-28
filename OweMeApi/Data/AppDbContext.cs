using Microsoft.EntityFrameworkCore;
using OweMeApi.Data.Entities;

namespace OweMeApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)           
                .WithMany()                    
                .HasForeignKey(u => u.RoleCode)
                .IsRequired();

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Code = "ADMIN", Label = "Administrator" },
                new UserRole { Code = "MODERATOR", Label = "Moderator" },
                new UserRole { Code = "USER", Label = "Użytkownik" },
                new UserRole { Code = "LOCKED", Label = "Zablokowany" }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}