using Application.Common;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.ChangeDebtApprovement;

public record ChangeDebtApprovementCommand(Guid DebtId, bool Approve) : IRequest<HandlerResult>;

public class ChangeDebtApprovementHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<ChangeDebtApprovementHandler> logger) : IRequestHandler<ChangeDebtApprovementCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeDebtApprovementCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtOwnerOnly(user)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Payment)
                    .ThenInclude(p => p!.StatusChanges)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        if (debt.GetIsSettled())
            return HandlerResult.Failure("Debt is settled", ErrorCode.BadRequest);

        if (debt.GetHasPendingPayments())
            return HandlerResult.Failure("Debt has pending payments", ErrorCode.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var creditorApproves = debt.GetCreditorApproves();

            var debtorApproves = debt.GetDebtorApproves();

            bool shouldSettle = false;

            // USER IS CREDITOR
            if (debt.CreditorId == user.Id)
            {
                var eventType = request.Approve ? LedgerEventTypes.CreditorDebtApprovement : LedgerEventTypes.CreditorDebtDisapprovement;

                var approvementEvent = debt.CreateApprovement(eventType);
                context.LedgerEvents.Add(approvementEvent);

                if (debtorApproves && eventType == LedgerEventTypes.CreditorDebtApprovement)
                    shouldSettle = true;

                if (creditorApproves && eventType == LedgerEventTypes.CreditorDebtApprovement)
                    return HandlerResult.Success();
            }

            // USER IS DEBTOR
            else if (debt.DebtorId == user.Id)
            {
                var eventType = request.Approve ? LedgerEventTypes.DebtorDebtApprovement : LedgerEventTypes.DebtorDebtDisapprovement;

                var approvementEvent = debt.CreateApprovement(eventType);
                context.LedgerEvents.Add(approvementEvent);

                if (creditorApproves && eventType == LedgerEventTypes.DebtorDebtApprovement)
                    shouldSettle = true;

                if (debtorApproves && eventType == LedgerEventTypes.DebtorDebtApprovement)
                    return HandlerResult.Success();
            }

            else
                return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

            if (shouldSettle)
            {
                var settlementEvent = debt.CreateSettlement();
                context.LedgerEvents.Add(settlementEvent);
            }

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return HandlerResult.Success();
        }
        catch (DebtIsSettledException exception)
        {
            logger.LogError(exception, "Change debt amount error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return HandlerResult.Failure("Debt is settled", ErrorCode.BadRequest);
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch { }

            logger.LogError(exception, "Change debt approvement error for {UserId}", user.Id);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
