using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtAmount;

public class ChangeDebtAmountHandler(AppDbContext context,
    ILogger<ChangeDebtAmountHandler> logger) : IRequestHandler<ChangeDebtAmountCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ChangeDebtAmountCommand request, CancellationToken ct)
    {
        if (!await context.Debts.AnyAsync(d => d.Id == request.DebtId && d.CreditorId == request.UserId, ct))
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

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
                ActorId = request.UserId,
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
            logger.LogError(exception, "Change debt amount error for {UserId}", request.UserId);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
