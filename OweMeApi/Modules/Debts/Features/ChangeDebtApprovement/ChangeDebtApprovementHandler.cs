using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Modules.Debts.Domain.Enums;
using OweMeApi.Modules.Debts.Filters;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtApprovement;

public class ChangeDebtApprovementHandler(
    AppDbContext context,
    DebtsService service,
    IUserContext user,
    ILogger<ChangeDebtApprovementHandler> logger) : IRequestHandler<ChangeDebtApprovementCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeDebtApprovementCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtOwnerOnly(user)
            .FirstOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        if (await service.GetDebtIsSettled(debt.Id, ct))
            return HandlerResult.Failure("Debt is settled", ErrorCode.BadRequest);

        if (await service.GetDebtHasPendingPayments(debt.Id, ct))
            return HandlerResult.Failure("Debt has pending payments", ErrorCode.BadRequest);

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            LedgerEvent approvementEvent = new()
            {
                DebtId = request.DebtId,
                ActorId = user.Id,
                InternalReference = LedgerEvent.GenReferenceNumber,
                EventType = string.Empty
            };

            var creditorApproves = await service.GetCreditorApproves(debt.Id, ct);

            var debtorApproves = await service.GetDebtorApproves(debt.Id, ct);

            bool shouldSettle = false;

            // USER IS CREDITOR
            if (debt.CreditorId == user.Id)
            {
                approvementEvent.EventType = request.Approve ? LedgerEventType.CreditorDebtApprovement : LedgerEventType.CreditorDebtDisapprovement;

                if (debtorApproves && approvementEvent.EventType == LedgerEventType.CreditorDebtApprovement)
                    shouldSettle = true;

                if (creditorApproves && approvementEvent.EventType == LedgerEventType.CreditorDebtApprovement)
                    return HandlerResult.Success();
            }

            // USER IS DEBTOR
            else if (debt.DebtorId == user.Id)
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
                    ActorId = user.Id,
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
            logger.LogError(exception, "Change debt approvement error for {UserId}", user.Id);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
