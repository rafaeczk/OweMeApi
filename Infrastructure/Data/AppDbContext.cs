using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Infrastructure.Data;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<FriendCode> FriendCodes => Set<FriendCode>();
    public DbSet<Friendship> Friendships => Set<Friendship>();

    // ---
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<LedgerEvent> LedgerEvents => Set<LedgerEvent>();
    public DbSet<DebtAdjustment> DebtAdjustments => Set<DebtAdjustment>();
    public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();
    public DbSet<DebtPaymentStatusChange> DebtPaymentStatusChanges => Set<DebtPaymentStatusChange>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return await Database.BeginTransactionAsync(ct);
    }
}

public static class EntityTypeBuilderExtensions
{
    public static void ConfigureAuditableEntryFields<T>(this EntityTypeBuilder<T> entity)
        where T : BaseAuditableEntity
    {
        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
