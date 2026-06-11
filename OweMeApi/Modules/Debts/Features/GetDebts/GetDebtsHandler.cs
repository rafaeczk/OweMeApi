using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Filters;
using OweMeApi.Modules.Debts.Domain.Enums;

namespace OweMeApi.Modules.Debts.Features.GetDebts;

public class GetDebtsHandler(
    AppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtsQuery, HandlerResult<List<DebtListItemDTO>>>
{
    public async Task<HandlerResult<List<DebtListItemDTO>>> Handle(GetDebtsQuery request, CancellationToken ct)
    {
        var debtsQuery = context.Debts.AsQueryable();

        debtsQuery = request.Role switch
        {
            QEUserRoleInDebt.Creditor => debtsQuery.DebtCreditorOnly(user),
            QEUserRoleInDebt.Debtor => debtsQuery.DebtDebtorOnly(user),
            _ => debtsQuery.DebtOwnerOnly(user),
        };

        debtsQuery = request.State switch
        {
            QEDebtState.Settled => debtsQuery.Where(d => d.LedgerEvents.Any(e => e.EventType == LedgerEventType.DebtSettlement)),
            QEDebtState.Unsettled => debtsQuery.Where(d => !d.LedgerEvents.Any(e => e.EventType == LedgerEventType.DebtSettlement)),
            _ => debtsQuery,
        };

        var debts = await debtsQuery.AsNoTracking().ToListAsync(ct);
        var debtIds = debts.Select(d => d.Id).ToList();

        var allEvents = await context.LedgerEvents
            .AsNoTracking()
            .Where(e => debtIds.Contains(e.DebtId))
            .Include(e => e.Adjustment)
            .Include(e => e.Payment)
                .ThenInclude(p => p.StatusChanges)
                    .ThenInclude(sc => sc.LedgerEvent)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(ct);

        var eventLookup = allEvents.ToLookup(e => e.DebtId);

        return debts.Select(d =>
        {
            var events = eventLookup[d.Id].ToList();

            return new DebtListItemDTO(
                d.Id,
                d.Title,
                d.Description,
                d.CreditorId,
                d.DebtorId,
                events.Where(e => e.EventType == LedgerEventType.Adjustment)
                    .OrderByDescending(e => e.Timestamp)
                    .Select(e => e.Adjustment!.Amount)
                    .FirstOrDefault(),
                events.Where(e => e.EventType == LedgerEventType.Payment)
                    .Select(e => e.Payment)
                    .Where(p => p != null)
                    .Where(p => p!.StatusChanges
                        .OrderByDescending(sc => sc.LedgerEvent.Timestamp)
                        .Select(sc => sc.Status)
                        .FirstOrDefault() == DebtPaymentStatus.Success)
                    .Select(p =>
                        (p!.PayerId == d.CreditorId && p!.ReceiverId == d.DebtorId)
                            ? -p!.Amount
                            : (p!.PayerId == d.DebtorId && p!.ReceiverId == d.CreditorId)
                                ? p!.Amount
                                : 0m)
                    .Sum(),
                events.Where(e =>
                    (e.EventType == LedgerEventType.CreditorDebtApprovement || e.EventType == LedgerEventType.CreditorDebtDisapprovement))
                    .OrderByDescending(e => e.Timestamp)
                    .Select(e => e.EventType == LedgerEventType.CreditorDebtApprovement)
                    .FirstOrDefault(),
                events.Where(e =>
                        (e.EventType == LedgerEventType.DebtorDebtApprovement || e.EventType == LedgerEventType.DebtorDebtDisapprovement))
                    .OrderByDescending(e => e.Timestamp)
                    .Select(e => e.EventType == LedgerEventType.DebtorDebtApprovement)
                    .FirstOrDefault(),
                events.Any(e => e.EventType == LedgerEventType.DebtSettlement)
            );
        }).ToList();

        //var result = await debtsQuery.Select(d => new DebtListItemDTO(
        //    d.Id,
        //    d.Title,
        //    d.Description,
        //    d.CreditorId,
        //    d.DebtorId,
        //    d.LedgerEvents.Where(e => e.EventType == LedgerEventType.Adjustment)
        //        .OrderByDescending(e => e.Timestamp)
        //        .Select(e => e.Adjustment!.Amount)
        //        .FirstOrDefault(),
        //    d.LedgerEvents.Where(e => e.EventType == LedgerEventType.Payment)
        //        .OrderByDescending(e => e.Timestamp)
        //        .Select(e => e.Payment)
        //        .Where(p => p != null)
        //        .Where(p => p!.StatusChanges
        //            .OrderByDescending(sc => sc.LedgerEvent.Timestamp)
        //            .Select(sc => (PaymentStatus?)sc.Status)
        //            .FirstOrDefault() == PaymentStatus.Success)
        //        .Select(p =>
        //            (p!.PayerId == d.CreditorId && p!.ReceiverId == d.DebtorId)
        //                ? -p!.Amount
        //                : (p!.PayerId == d.DebtorId && p!.ReceiverId == d.CreditorId)
        //                    ? p!.Amount
        //                    : 0m)
        //        .Sum(),
        //    d.LedgerEvents.Where(e => e.EventType == LedgerEventType.CreditorDebtApprovement || e.EventType == LedgerEventType.CreditorDebtDisapprovement)
        //        .OrderByDescending(e => e.Timestamp).Select(e => e.EventType == LedgerEventType.CreditorDebtApprovement).FirstOrDefault(),
        //    d.LedgerEvents.Where(e => e.EventType == LedgerEventType.DebtorDebtApprovement || e.EventType == LedgerEventType.DebtorDebtDisapprovement)
        //        .OrderByDescending(e => e.Timestamp).Select(e => e.EventType == LedgerEventType.DebtorDebtApprovement).FirstOrDefault(),
        //    d.LedgerEvents.Any(e => e.EventType == LedgerEventType.DebtSettlement)
        //)).ToListAsync(ct);

        //return result;
    }
}
