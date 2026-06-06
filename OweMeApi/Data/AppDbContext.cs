using Microsoft.EntityFrameworkCore;
using OweMeApi.Data.Entities;

namespace OweMeApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<FriendCode> FriendCodes => Set<FriendCode>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureUserRole(modelBuilder);
        ConfigureFriendCode(modelBuilder);
        ConfigureFriendship(modelBuilder);
        ConfigureDebt(modelBuilder);
        ConfigurateLedgerEntry(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Id)
                  .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(u => u.Role)
                  .WithMany()
                  .HasForeignKey(u => u.RoleCode)
                  .IsRequired();
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Code = "ADMIN", Label = "Administrator" },
            new UserRole { Code = "MODERATOR", Label = "Moderator" },
            new UserRole { Code = "USER", Label = "Użytkownik" },
            new UserRole { Code = "LOCKED", Label = "Zablokowany" }
        );
    }

    private static void ConfigureFriendCode(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FriendCode>(entity =>
        {
            entity.HasKey(fc => fc.UserId);

            entity.Property(fc => fc.Code)
                  .IsRequired()
                  .HasMaxLength(6);

            entity.HasIndex(fc => fc.Code)
                  .IsUnique();

            entity.HasOne<User>()
                  .WithOne()
                  .HasForeignKey<FriendCode>(fc => fc.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureFriendship(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(f => new { f.UserId, f.FriendId });

            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDebt(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasOne(d => d.Creditor)
                .WithMany()
                .HasForeignKey(d => d.CreditorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Debtor)
                .WithMany()
                .HasForeignKey(d => d.DebtorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurateLedgerEntry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LedgerEntry>(entity =>
        {
            entity.Property(e => e.TransactionType)
                .HasConversion<string>();

            entity.Property(e => e.PaymentStatus)
                .HasConversion<string>();

            entity.Property(e => e.PaymentMethod)
                .HasConversion<string>();

            entity.HasOne(e => e.Debt)
                .WithMany()
                .HasForeignKey(e => e.DebtId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.InternalReference)
                .IsUnique();

        });
    }
}
