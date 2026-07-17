using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Common;
using Domain.Exceptions;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.ChangeDebtAmount;

public record ChangeDebtAmountCommand(Guid DebtId, decimal Amount, string Note) : IRequest<Result>;

public class ChangeDebtAmountHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<ChangeDebtAmountHandler> logger) : IRequestHandler<ChangeDebtAmountCommand, Result>
{
    public async Task<Result> Handle(ChangeDebtAmountCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtCreditorOnly(user)
            .Include(d => d.LedgerEvents)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

        if (debt.GetIsSettled())
            return Result.Failure("Debt is settled", FailureReason.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var adjustment = debt.CreateAdjustment(new Money(request.Amount), request.Note);

            context.DebtAdjustments.Add(adjustment);
            context.LedgerEvents.Add(adjustment.LedgerEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success();
        }
        catch(DebtIsSettledException exception)
        {
            logger.LogError(exception, "Change debt amount error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Debt is settled", FailureReason.BadRequest);
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch { }

            logger.LogError(exception, "Change debt amount error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Technical error", FailureReason.InternalError);
        }
    }
}
