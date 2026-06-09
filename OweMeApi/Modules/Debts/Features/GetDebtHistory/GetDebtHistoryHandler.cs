using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Modules.Debts.Dtos;

namespace OweMeApi.Modules.Debts.Features.GetDebtHistory;

public class GetDebtHistoryHandler(AppDbContext context) : IRequestHandler<GetDebtHistoryQuery, HandlerResult<List<DebtHistoryListItemDTO>>>
{
    private readonly List<LedgerEventType> allowedEventTypes =
    [
        LedgerEventType.Payment,
        LedgerEventType.Adjustment,
        LedgerEventType.DebtSettlement,
        LedgerEventType.CreditorDebtApprovement, LedgerEventType.CreditorDebtDisapprovement,
        LedgerEventType.DebtorDebtApprovement, LedgerEventType.DebtorDebtDisapprovement
    ];

    public async Task<HandlerResult<List<DebtHistoryListItemDTO>>> Handle(GetDebtHistoryQuery request, CancellationToken ct)
    {
        var debtQuery = context.Debts.Where(d => d.Id == request.DebtId && (d.CreditorId == request.UserId || d.DebtorId == request.UserId));

        if (!await debtQuery.AnyAsync(ct))
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        var debtEventsQuery = await debtQuery
            .SelectMany(d => d.LedgerEvents)
            .Where(e => allowedEventTypes.Contains(e.EventType))
            .OrderBy(e => e.Timestamp)
            .Include(e => e.Adjustment)
            .Include(e => e.Payment)
                .ThenInclude(p => p!.StatusChanges)
                    .ThenInclude(sc => sc.LedgerEvent)
            .ToListAsync(ct);

        return debtEventsQuery.Select(e =>
        {
            var statusChanges = e.Payment?.StatusChanges.OrderBy(sc => sc.LedgerEvent.Timestamp);

            return new DebtHistoryListItemDTO(
                e.Id,
                e.EventType,
                e.InternalReference,
                e.Timestamp,
                e.ActorId,
                e.Adjustment != null
                    ? new DebtHistoryListItemAdjustmentDTO(
                        e.Adjustment.Id,
                        e.Adjustment.Amount,
                        e.Adjustment.Note)
                    : null,
                e.Payment != null
                    ? new DebtHistoryListItemPaymentDTO(
                        e.Payment.Id,
                        e.Payment.Amount,
                        e.Payment.PayerId,
                        e.Payment.ReceiverId,
                        e.Payment.Method,
                        e.Payment.Note,
                        statusChanges!.Select(sc => sc.Status).Last(),
                        [.. statusChanges
                            !.Select(sc => new DebtHistoryListItemPaymentHistoryListItemDTO(
                                sc.LedgerEvent.Id,
                                sc.LedgerEvent.EventType,
                                sc.LedgerEvent.InternalReference,
                                sc.LedgerEvent.Timestamp,
                                sc.LedgerEvent.ActorId,
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
