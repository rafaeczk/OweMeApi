using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Modules.Debts.Dtos;

namespace OweMeApi.Modules.Debts;

public class DebtsService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<(decimal, decimal)> CalcDebtTotals(Debt debt)
    {
        decimal totalAmount = debt.Amount + await _context.LedgerEntries
            .Where(e => e.DebtId == debt.Id && e.TransactionType == TransactionType.Adjustment)
            .SumAsync(e => (decimal?)e.Amount) ?? 0;

        decimal totalPayments = await _context.LedgerEntries
            .Where(e => e.DebtId == debt.Id && e.TransactionType == TransactionType.Payment && e.PaymentStatus == PaymentStatus.Confirmed)
            .SumAsync(e => (decimal?)e.Amount) ?? 0;

        return (totalAmount, totalPayments);
    }

    public async Task<(decimal, decimal, DebtSummaryDTO)> CalcDebtSummary(Debt debt)
    {
        var (totalAmount, totalPayments) = await CalcDebtTotals(debt);

        decimal amount = totalAmount - totalPayments;

        DebtSummaryDTO summary;

        if (amount > 0)
            summary = new DebtSummaryDTO(debt.DebtorId, debt.CreditorId, Math.Abs(amount));
        else if (amount < 0)
            summary = new DebtSummaryDTO(debt.CreditorId, debt.DebtorId, Math.Abs(amount));
        else
            summary = new DebtSummaryDTO(Guid.Empty, Guid.Empty, 0m);

        return (totalAmount, totalPayments, summary);
    }
}
