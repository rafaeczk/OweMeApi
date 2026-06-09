using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.CreateDebt;

public class CreateDebtHandler(
    AppDbContext context,
    ILogger<CreateDebtHandler> logger,
    IUserContext user) : IRequestHandler<CreateDebtCommand, HandlerResult<Guid>>
{
    public async Task<HandlerResult<Guid>> Handle(CreateDebtCommand request, CancellationToken ct)
    {
        if (!await context.Users.AnyAsync(u => u.Id == request.DebtorId, ct))
            return HandlerResult.Failure("Debtor not found", ErrorCode.NotFound);

        if (user.Id == request.DebtorId)
            return HandlerResult.Failure("You cannot debt yourself", ErrorCode.BadRequest);

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            Debt debt = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CreditorId = user.Id,
                DebtorId = request.DebtorId
            };
            context.Debts.Add(debt);

            DebtAdjustment adjustment = new()
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Note = "Initial debt amount"
            };
            context.DebtAdjustments.Add(adjustment);

            LedgerEvent adjustmentEvent = new()
            {
                DebtId = debt.Id,
                ActorId = user.Id,
                EventType = LedgerEventType.Adjustment,
                AdjustmentId = adjustment.Id,
                InternalReference = LedgerEvent.GenReferenceNumber
            };
            context.LedgerEvents.Add(adjustmentEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return debt.Id;
        }
        catch(Exception exception)
        {
            logger.LogError(exception, "Create debt error for {DebtorId}", request.DebtorId);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
