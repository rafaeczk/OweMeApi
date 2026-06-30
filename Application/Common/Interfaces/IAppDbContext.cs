using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<FriendCode> FriendCodes { get; }
    DbSet<Friendship> Friendships { get; }
    DbSet<Debt> Debts { get; }
    DbSet<LedgerEvent> LedgerEvents { get; }
    DbSet<DebtAdjustment> DebtAdjustments { get; }
    DbSet<DebtPayment> DebtPayments { get; }
    DbSet<DebtPaymentStatusChange> DebtPaymentStatusChanges { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);

    T Entry<T>(T entity);
}
