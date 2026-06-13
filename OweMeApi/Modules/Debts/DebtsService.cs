using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Modules.Debts.Domain.Enums;
using OweMeApi.Modules.Debts.Features.GetDebt;

namespace OweMeApi.Modules.Debts;

public class DebtsService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<decimal> GetTotalAmount(Guid debtId, CancellationToken ct)
    {
        return await _context.LedgerEvents
            .Where(e => e.DebtId == debtId && e.EventType == LedgerEventType.Adjustment)
            .OrderByDescending(e => e.Timestamp)
            .Select(e => e.Adjustment!.Amount)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<decimal> GetTotalPayments(Guid debtId, CancellationToken ct)
    {
        return await _context.LedgerEvents
            .Where(e => e.DebtId == debtId && e.EventType == LedgerEventType.Payment)
            .OrderByDescending(e => e.Timestamp)
            .Select(e => e.Payment)
            .Where(p => p != null)
            .Where(p => p!.StatusChanges
                .OrderByDescending(sc => sc.LedgerEvent.Timestamp)
                .Select(sc => sc.Status)
                .FirstOrDefault() == DebtPaymentStatus.Success)
            .Select(p =>
                (p!.PayerId == p.LedgerEvent.Debt.CreditorId && p!.ReceiverId == p.LedgerEvent.Debt.DebtorId)
                    ? -p!.Amount
                    : (p!.PayerId == p.LedgerEvent.Debt.DebtorId && p!.ReceiverId == p.LedgerEvent.Debt.CreditorId)
                        ? p!.Amount
                        : 0m)
            .SumAsync(ct);
    }

    public DebtSummaryDTO GetSummary(decimal totalAmount, decimal totalPayments, Guid creditorId, Guid debtorId)
    {
        var diff = totalAmount - totalPayments;

        if (diff > 0)
            return new DebtSummaryDTO(debtorId, creditorId, Math.Abs(diff));
        else if (diff < 0)
            return new DebtSummaryDTO(creditorId, debtorId, Math.Abs(diff));
        else
            return new DebtSummaryDTO(Guid.Empty, Guid.Empty, 0m);
    }

    public async Task<bool> GetCreditorApproves(Guid debtId, CancellationToken ct)
    {
        return await _context.LedgerEvents
            .Where(e =>
                (e.EventType == LedgerEventType.CreditorDebtApprovement || e.EventType == LedgerEventType.CreditorDebtDisapprovement)
                && e.DebtId == debtId)
            .OrderByDescending(e => e.Timestamp)
            .Select(e => e.EventType == LedgerEventType.CreditorDebtApprovement)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> GetDebtorApproves(Guid debtId, CancellationToken ct)
    {
        return await _context.LedgerEvents
            .Where(e =>
                (e.EventType == LedgerEventType.DebtorDebtApprovement || e.EventType == LedgerEventType.DebtorDebtDisapprovement)
                && e.DebtId == debtId)
            .OrderByDescending(e => e.Timestamp)
            .Select(e => e.EventType == LedgerEventType.DebtorDebtApprovement)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> GetDebtIsSettled(Guid debtId, CancellationToken ct)
    {
        return await _context.LedgerEvents
            .AnyAsync(e => e.DebtId == debtId && e.EventType == LedgerEventType.DebtSettlement, ct);
    }

    public async Task<bool> GetDebtHasPendingPayments(Guid debtId, CancellationToken ct)
    {
        return await context.DebtPayments
            .Where(p => p.LedgerEvent.DebtId == debtId)
            .AnyAsync(p => p.StatusChanges
                .OrderByDescending(sc => sc.LedgerEvent.Timestamp)
                .Take(1)
                .Any(sc => sc.Status == DebtPaymentStatus.Pending), ct);
    }
}
