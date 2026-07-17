using Application.Common.Interfaces;
using Application.Common.Pagination;
using Application.Modules.Debts._Filters;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Debts.GetDebts;

public enum QEUserRoleInDebt
{
    Debtor, Creditor, Any
}

public enum QEDebtState
{
    Settled, Unsettled, Any
}

public record GetDebtsQuery(QEUserRoleInDebt Role, QEDebtState State, PaginationParams Pagination) : PaginationParams(Pagination), IRequest<Result<PagedResult<DebtListItemDTO>>>;

public class GetDebtsHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtsQuery, Result<PagedResult<DebtListItemDTO>>>
{
    public async Task<Result<PagedResult<DebtListItemDTO>>> Handle(GetDebtsQuery request, CancellationToken ct)
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
            QEDebtState.Settled => debtsQuery.Where(d => d.LedgerEvents.Any(e => e.EventType == LedgerEventTypes.DebtSettlement)),
            QEDebtState.Unsettled => debtsQuery.Where(d => !d.LedgerEvents.Any(e => e.EventType == LedgerEventTypes.DebtSettlement)),
            _ => debtsQuery,
        };

        var totalDebts = await debtsQuery.CountAsync(ct);

        var debts = await debtsQuery
            .Paginate(request)
            .AsNoTracking()
            .ToListAsync(ct);

        var debtIds = debts.Select(d => d.Id).ToList();

        var allEvents = await context.LedgerEvents
            .AsNoTracking()
            .Where(e => debtIds.Contains(e.DebtId))
            .Include(e => e.Adjustment)
            .Include(e => e.Payment)
                .ThenInclude(p => p!.StatusChanges)
                    .ThenInclude(sc => sc.LedgerEvent)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        var eventLookup = allEvents.ToLookup(e => e.DebtId);

        var debtDTOs = debts.Select(d =>
        {
            var events = eventLookup[d.Id].ToList();

            return new DebtListItemDTO(
                d.Id,
                d.Title,
                d.Description,
                d.CreditorId,
                d.DebtorId,
                d.CreatedAt,
                events.Where(e => e.EventType == LedgerEventTypes.Adjustment)
                    .OrderByDescending(e => e.CreatedAt)
                    .Select(e => e.Adjustment!.Money.Amount)
                    .FirstOrDefault(),
                events.Where(e => e.EventType == LedgerEventTypes.Payment)
                    .Select(e => e.Payment)
                    .Where(p => p != null)
                    .Where(p => p!.StatusChanges
                        .OrderByDescending(e => e.LedgerEvent.CreatedAt)
                        .Select(e => e.Status)
                        .FirstOrDefault() == DebtPaymentStatus.Success)
                    .Sum(p =>
                        (p!.PayerId == d.CreditorId && p!.ReceiverId == d.DebtorId)
                            ? -p!.Money.Amount
                            : (p!.PayerId == d.DebtorId && p!.ReceiverId == d.CreditorId)
                                ? p!.Money.Amount
                                : 0m),
                events.Where(e =>
                    (e.EventType == LedgerEventTypes.CreditorDebtApprovement || e.EventType == LedgerEventTypes.CreditorDebtDisapprovement))
                    .OrderByDescending(e => e.CreatedAt)
                    .Select(e => e.EventType == LedgerEventTypes.CreditorDebtApprovement)
                    .FirstOrDefault(),
                events.Where(e =>
                        (e.EventType == LedgerEventTypes.DebtorDebtApprovement || e.EventType == LedgerEventTypes.DebtorDebtDisapprovement))
                    .OrderByDescending(e => e.CreatedAt)
                    .Select(e => e.EventType == LedgerEventTypes.DebtorDebtApprovement)
                    .FirstOrDefault(),
                events.Any(e => e.EventType == LedgerEventTypes.DebtSettlement)
            );
        }).ToList();

        return new PagedResult<DebtListItemDTO>(debtDTOs, totalDebts, request);
    }
}
