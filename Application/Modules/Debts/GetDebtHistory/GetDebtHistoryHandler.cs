using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Enums;

namespace Application.Modules.Debts.GetDebtHistory;

public record GetDebtHistoryQuery(Guid DebtId) : IRequest<HandlerResult<List<DebtHistoryListItemDTO>>>;

public class GetDebtHistoryHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtHistoryQuery, HandlerResult<List<DebtHistoryListItemDTO>>>
{
    private readonly List<string> allowedEventTypes =
    [
        LedgerEventTypes.Payment,
        LedgerEventTypes.Adjustment,
        LedgerEventTypes.DebtSettlement,
        LedgerEventTypes.CreditorDebtApprovement, LedgerEventTypes.CreditorDebtDisapprovement,
        LedgerEventTypes.DebtorDebtApprovement, LedgerEventTypes.DebtorDebtDisapprovement
    ];

    public async Task<HandlerResult<List<DebtHistoryListItemDTO>>> Handle(GetDebtHistoryQuery request, CancellationToken ct)
    {
        var debtQuery = context.Debts
            .DebtOwnerOnly(user)
            .Where(d => d.Id == request.DebtId);

        if (!await debtQuery.AnyAsync(ct))
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        var debtEvents = await debtQuery
            .SelectMany(d => d.LedgerEvents)
            .Where(e => allowedEventTypes.Contains(e.EventType))
            .OrderBy(e => e.CreatedAt)
            .Include(e => e.Adjustment)
            .Include(e => e.Payment)
                .ThenInclude(p => p!.StatusChanges)
                    .ThenInclude(sc => sc.LedgerEvent)
            .ToListAsync(ct);

        return debtEvents.Select(e =>
        {
            var statusChanges = e.Payment?.StatusChanges.OrderBy(sc => sc.LedgerEvent.CreatedAt);

            return new DebtHistoryListItemDTO(
                e.Id,
                e.EventType,
                e.InternalReference,
                e.CreatedAt,
                e.CreatedBy,
                e.Adjustment != null
                    ? new DebtHistoryListItemAdjustmentDTO(
                        e.Adjustment.Id,
                        e.Adjustment.Money.Amount,
                        e.Adjustment.Note)
                    : null,
                e.Payment != null
                    ? new DebtHistoryListItemPaymentDTO(
                        e.Payment.Id,
                        e.Payment.Money.Amount,
                        e.Payment.PayerId,
                        e.Payment.ReceiverId,
                        e.Payment.Method,
                        e.Payment.Note,
                        statusChanges!.Select(sc => sc.Status).Last(),
                        [.. statusChanges
                            !.Select(sc => new DebtHistoryListItemPaymentHistoryListItemDTO(
                                sc.Id,
                                sc.LedgerEvent.EventType,
                                sc.LedgerEvent.InternalReference,
                                sc.LedgerEvent.CreatedAt,
                                sc.LedgerEvent.CreatedBy,
                                new DebtHistoryListItemPaymentStatusChangeDTO(
                                    sc.Id,
                                    sc.Status,
                                    sc.Note)))
                        ])
                    : null);
        })
            .ToList();
    }
}
