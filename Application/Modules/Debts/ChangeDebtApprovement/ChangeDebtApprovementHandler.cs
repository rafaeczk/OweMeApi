using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.ChangeDebtApprovement;

public record ChangeDebtApprovementCommand(Guid DebtId, bool Approve) : IRequest<Result>;

public class ChangeDebtApprovementHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<ChangeDebtApprovementHandler> logger) : IRequestHandler<ChangeDebtApprovementCommand, Result>
{
    public async Task<Result> Handle(ChangeDebtApprovementCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtParticipantOnly(user)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Payment)
                    .ThenInclude(p => p!.StatusChanges)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

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

                var approvementEvent = debt.CreateApprovement(user.Id, eventType);
                context.LedgerEvents.Add(approvementEvent);

                if (debtorApproves && eventType == LedgerEventTypes.CreditorDebtApprovement)
                    shouldSettle = true;

                if (creditorApproves && eventType == LedgerEventTypes.CreditorDebtApprovement)
                    return Result.Success();
            }

            // USER IS DEBTOR
            else if (debt.DebtorId == user.Id)
            {
                var eventType = request.Approve ? LedgerEventTypes.DebtorDebtApprovement : LedgerEventTypes.DebtorDebtDisapprovement;

                var approvementEvent = debt.CreateApprovement(user.Id, eventType);
                context.LedgerEvents.Add(approvementEvent);

                if (creditorApproves && eventType == LedgerEventTypes.DebtorDebtApprovement)
                    shouldSettle = true;

                if (debtorApproves && eventType == LedgerEventTypes.DebtorDebtApprovement)
                    return Result.Success();
            }

            else
                return Result.Failure("Debt not found", FailureReason.NotFound);

            if (shouldSettle)
            {
                var settlementEvent = debt.CreateSettlement();
                context.LedgerEvents.Add(settlementEvent);
            }

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success();
        }
        catch (UnauthorizedDebtAccessException exception)
        {
            logger.LogError(exception, "Change debt approvement error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Unauthorized", FailureReason.Unauthorized);
        }
        catch (DebtIsSettledException exception)
        {
            logger.LogError(exception, "Change debt approvement error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Debt is settled", FailureReason.BadRequest);
        }
        catch (DebtHasPendingPaymentsException exception)
        {
            logger.LogError(exception, "Change debt approvement error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Debt has pending payments", FailureReason.BadRequest);
        }
        catch (DebtIsNotFullyApprovedException exception)
        {
            logger.LogError(exception, "Change debt approvement error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Debt is not fully approved to be settled", FailureReason.BadRequest);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Change debt approvement error for {UserId}", user.Id);

            return Result.Failure("Technical error", FailureReason.InternalError);
        }
    }
}
