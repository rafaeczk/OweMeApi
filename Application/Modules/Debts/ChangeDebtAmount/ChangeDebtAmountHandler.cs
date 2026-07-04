using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Application.Modules.Debts._Filters;
using Domain.ValueObjects;
using Domain.Exceptions;

namespace Application.Modules.Debts.ChangeDebtAmount;

public record ChangeDebtAmountCommand(Guid DebtId, decimal Amount, string Note) : IRequest<HandlerResult>;

public class ChangeDebtAmountHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<ChangeDebtAmountHandler> logger) : IRequestHandler<ChangeDebtAmountCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeDebtAmountCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtCreditorOnly(user)
            .Include(d => d.LedgerEvents)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        if (debt.GetIsSettled())
            return HandlerResult.Failure("Debt is settled", ErrorCode.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var adjustment = debt.CreateAdjustment(new Money(request.Amount), request.Note);

            context.DebtAdjustments.Add(adjustment);
            context.LedgerEvents.Add(adjustment.LedgerEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return HandlerResult.Success();
        }
        catch(DebtIsSettledException exception)
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

            logger.LogError(exception, "Change debt amount error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
