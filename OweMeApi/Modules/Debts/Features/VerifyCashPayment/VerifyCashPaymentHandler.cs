using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public class VerifyCashPaymentHandler(
    AppDbContext context,
    ILogger<VerifyCashPaymentHandler> logger) : IRequestHandler<VerifyCashPaymentCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(VerifyCashPaymentCommand request, CancellationToken ct)
    {
        var paymentEvent = await context.LedgerEvents
            .Include(e => e.Payment)
            .FirstOrDefaultAsync(e => e.Id == request.LedgerEventId && e.EventType == LedgerEventType.Payment, ct);

        if (paymentEvent == null)
            return HandlerResult.Failure("Payment not found", ErrorCode.NotFound);

        if (paymentEvent.PaymentId == null || paymentEvent.Payment == null)
            return HandlerResult.Failure("Payment is missing in event", ErrorCode.NotFound);

        if (paymentEvent.Payment.ReceiverId != request.UserId)
            return HandlerResult.Failure("You cannot verify this payment", ErrorCode.Unauthorized);

        var currentPaymentStatus = await context.DebtPaymentStatusChanges
            .Where(psc => psc.PaymentId == paymentEvent.Payment.Id)
            .OrderByDescending(psc => psc.LedgerEvent.Timestamp)
            .Select(psc => psc.Status)
            .FirstOrDefaultAsync(ct);

        if (currentPaymentStatus != PaymentStatus.Pending)
            return HandlerResult.Failure("You have already verified this cash payment", ErrorCode.BadRequest);

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            DebtPaymentStatusChange statusChange = new()
            {
                Id = Guid.NewGuid(),
                PaymentId = paymentEvent.PaymentId.Value,
                Status = request.Status,
                Note = request.Note
            };
            context.DebtPaymentStatusChanges.Add(statusChange);

            LedgerEvent statusChangeEvent = new()
            {
                Id = Guid.NewGuid(),
                DebtId = paymentEvent.DebtId,
                ActorId = request.UserId,
                EventType = LedgerEventType.PaymentStatusChange,
                PaymentStatusChangeId = statusChange.Id,
                InternalReference = LedgerEvent.GenReferenceNumber
            };
            context.LedgerEvents.Add(statusChangeEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return HandlerResult.Success();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Payment verification error for {UserId}", request.UserId);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
