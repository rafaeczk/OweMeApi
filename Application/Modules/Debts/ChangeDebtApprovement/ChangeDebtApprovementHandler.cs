using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Application.Modules.Debts._Filters;
using Domain.Entities;
using Domain.Enums;

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
            .FirstOrDefaultAsync(d => d.Id == request.DebtId, ct);

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

                debt.CreateApprovement(eventType);

                if (debtorApproves && eventType == LedgerEventTypes.CreditorDebtApprovement)
                    shouldSettle = true;

                if (creditorApproves && eventType == LedgerEventTypes.CreditorDebtApprovement)
                    return HandlerResult.Success();
            }

            // USER IS DEBTOR
            else if (debt.DebtorId == user.Id)
            {
                var eventType = request.Approve ? LedgerEventTypes.DebtorDebtApprovement : LedgerEventTypes.DebtorDebtDisapprovement;

                debt.CreateApprovement(eventType);

                if (creditorApproves && eventType == LedgerEventTypes.DebtorDebtApprovement)
                    shouldSettle = true;

                if (debtorApproves && eventType == LedgerEventTypes.DebtorDebtApprovement)
                    return HandlerResult.Success();
            }

            else
                return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

            if (shouldSettle)
                debt.CreateSettlement();

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
