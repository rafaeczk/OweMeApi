using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Modules.Debts.Domain.Enums;
using OweMeApi.Modules.Debts.Filters;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtAmount;

public class ChangeDebtAmountHandler(
    AppDbContext context,
    DebtsService service,
    IUserContext user,
    ILogger<ChangeDebtAmountHandler> logger) : IRequestHandler<ChangeDebtAmountCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeDebtAmountCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtCreditorOnly(user)
            .FirstOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        if (await service.GetDebtIsSettled(debt.Id, ct))
            return HandlerResult.Failure("Debt is settled", ErrorCode.BadRequest);

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            DebtAdjustment adjustment = new()
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Note = request.Note
            };
            context.DebtAdjustments.Add(adjustment);

            LedgerEvent adjustmentEvent = new()
            {
                DebtId = request.DebtId,
                ActorId = user.Id,
                EventType = LedgerEventType.Adjustment,
                AdjustmentId = adjustment.Id,
                InternalReference = LedgerEvent.GenReferenceNumber
            };
            context.LedgerEvents.Add(adjustmentEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return HandlerResult.Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Change debt amount error for {UserId}", user.Id);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
