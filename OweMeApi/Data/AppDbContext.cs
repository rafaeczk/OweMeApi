using Microsoft.EntityFrameworkCore;
using OweMeApi.Data.Entities;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<FriendCode> FriendCodes => Set<FriendCode>();
    public DbSet<Friendship> Friendships => Set<Friendship>();

    // ---
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<LedgerEvent> LedgerEvents => Set<LedgerEvent>();
    public DbSet<DebtAdjustment> DebtAdjustments => Set<DebtAdjustment>();
    public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();
    public DbSet<DebtPaymentStatusChange> DebtPaymentStatusChanges => Set<DebtPaymentStatusChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureUserRole(modelBuilder);
        ConfigureFriendCode(modelBuilder);
        ConfigureFriendship(modelBuilder);
        ConfigureLedgerEvent(modelBuilder);
        ConfigureDebtPayment(modelBuilder);
        ConfigureDebtPaymentStatusChange(modelBuilder);
        ConfigureDebtAdjustment(modelBuilder);
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

    private static void ConfigureLedgerEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LedgerEvent>(entity =>
        {
            entity.Property(e => e.EventType)
                .HasConversion<string>();

            entity.HasIndex(e => e.InternalReference)
                .IsUnique();

            entity.HasOne(e => e.Debt)
                .WithMany(d => d.LedgerEvents)
                .HasForeignKey(e => e.DebtId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Actor)
                .WithMany()
                .HasForeignKey(e => e.ActorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // DETAILS

            entity.HasOne(e => e.Adjustment)
                .WithOne(a => a.LedgerEvent)
                .HasForeignKey<LedgerEvent>(e => e.AdjustmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Payment)
                .WithOne(p => p.LedgerEvent)
                .HasForeignKey<LedgerEvent>(e => e.PaymentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PaymentStatusChange)
                .WithOne(psc => psc.LedgerEvent)
                .HasForeignKey<LedgerEvent>(e => e.PaymentStatusChangeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDebtAdjustment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DebtAdjustment>(entity =>
        {
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);
        });
    }

    private static void ConfigureDebtPayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DebtPayment>(entity =>
        {
            entity.Property(e => e.Method)
                .HasConversion<string>();

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);
        });
    }

    private static void ConfigureDebtPaymentStatusChange(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DebtPaymentStatusChange>(entity =>
        {
            entity.Property(e => e.Status)
                .HasConversion<string>();
        });
    }
}
