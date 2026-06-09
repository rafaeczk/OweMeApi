using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;
using OweMeApi.Filters;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public class VerifyCashPaymentHandler(
    AppDbContext context,
    IUserContext user,
    ILogger<VerifyCashPaymentHandler> logger) : IRequestHandler<VerifyCashPaymentCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(VerifyCashPaymentCommand request, CancellationToken ct)
    {
        var payment = await context.DebtPayments
            .DebtPaymentPayerOnly(user)
            .Include(p => p.LedgerEvent)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, ct);

        if (payment == null)
            return HandlerResult.Failure("Payment not found", ErrorCode.NotFound);

        var currentPaymentStatus = await context.DebtPaymentStatusChanges
            .Where(psc => psc.PaymentId == payment.Id)
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
                PaymentId = payment.Id,
                Status = request.Status,
                Note = request.Note
            };
            context.DebtPaymentStatusChanges.Add(statusChange);

            LedgerEvent statusChangeEvent = new()
            {
                Id = Guid.NewGuid(),
                DebtId = payment.LedgerEvent.DebtId,
                ActorId = user.Id,
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
            logger.LogError(exception, "Payment verification error for {UserId}", user.Id);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
