using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities;

namespace OweMeApi.Modules.LedgerEntries;

public class LedgerEntriesService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<decimal> CalcCurrentDebtAmount(Debt debt, DateTime? to = null)
    {
        if (to == null)
            to = DateTime.UtcNow;

        return debt.Amount + await _context.LedgerEntries
            .Where(e => e.DebtId == debt.Id && e.TransactionType == TransactionType.Adjustment && e.CreatedAt <= to)
            .SumAsync(e => (decimal?)e.Amount) ?? 0;
    }

    public async Task<decimal> CalcAdjustmentsSum(Guid debtId)
    {
        return await _context.LedgerEntries
            .Where(e => e.DebtId == debtId && e.TransactionType == TransactionType.Adjustment)
            .SumAsync(e => (decimal?)e.Amount) ?? 0;
    }
}
