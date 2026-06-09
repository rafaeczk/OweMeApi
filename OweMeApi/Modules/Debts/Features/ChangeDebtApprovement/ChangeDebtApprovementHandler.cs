using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtApprovement;

public class ChangeDebtApprovementHandler(
    AppDbContext context,
    DebtsService service,
    ILogger<ChangeDebtApprovementHandler> logger) : IRequestHandler<ChangeDebtApprovementCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeDebtApprovementCommand request, CancellationToken ct)
    {
        var debt = await context.Debts.FirstOrDefaultAsync(d => d.Id == request.DebtId && (d.CreditorId == request.UserId || d.DebtorId == request.UserId), ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        if (await CheckIsSettled(debt.Id))
            return HandlerResult.Failure("Debt is settled", ErrorCode.BadRequest);

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            LedgerEvent approvementEvent = new()
            {
                DebtId = request.DebtId,
                ActorId = request.UserId,
                InternalReference = LedgerEvent.GenReferenceNumber
            };

            var creditorApproves = await service.GetCreditorApproves(debt.Id, ct);

            var debtorApproves = await service.GetDebtorApproves(debt.Id, ct);

            bool shouldSettle = false;

            // USER IS CREDITOR
            if (debt.CreditorId == request.UserId)
            {
                approvementEvent.EventType = request.Approve ? LedgerEventType.CreditorDebtApprovement : LedgerEventType.CreditorDebtDisapprovement;

                if (debtorApproves && approvementEvent.EventType == LedgerEventType.CreditorDebtApprovement)
                    shouldSettle = true;

                if (creditorApproves && approvementEvent.EventType == LedgerEventType.CreditorDebtApprovement)
                    return HandlerResult.Success();
            }

            // USER IS DEBTOR
            else if (debt.DebtorId == request.UserId)
            {
                approvementEvent.EventType = request.Approve ? LedgerEventType.DebtorDebtApprovement : LedgerEventType.DebtorDebtDisapprovement;

                if (creditorApproves && approvementEvent.EventType == LedgerEventType.DebtorDebtApprovement)
                    shouldSettle = true;

                if (debtorApproves && approvementEvent.EventType == LedgerEventType.DebtorDebtApprovement)
                    return HandlerResult.Success();
            }

            else
                return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

            context.LedgerEvents.Add(approvementEvent);

            if (shouldSettle)
            {
                LedgerEvent settlementEvent = new()
                {
                    DebtId = request.DebtId,
                    ActorId = request.UserId,
                    EventType = LedgerEventType.DebtSettlement,
                    InternalReference = LedgerEvent.GenReferenceNumber
                };

                context.LedgerEvents.Add(settlementEvent);
            }

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return HandlerResult.Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Change debt approvement error for {UserId}", request.UserId);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }

    private async Task<bool> CheckIsSettled(Guid debtId)
    {
        return await context.LedgerEvents.Where(e => e.DebtId == debtId && e.EventType == LedgerEventType.DebtSettlement).AnyAsync();
    }
}
