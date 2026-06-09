using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.CreatePayment;

public class CreatePaymentHandler(
    AppDbContext context,
    ILogger<CreatePaymentHandler> logger) : IRequestHandler<CreatePaymentCommand, HandlerResult<Guid>>
{
    public async Task<HandlerResult<Guid>> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var debt = await context.Debts.FirstOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        if(debt.CreditorId != request.UserId && debt.DebtorId != request.UserId)
            return HandlerResult.Failure("User does not have access to this debt", ErrorCode.Unauthorized);

        using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            DebtPayment payment = new()
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                PayerId = request.UserId,
                ReceiverId = (debt.CreditorId == request.UserId) ? debt.DebtorId : debt.CreditorId,
                Method = request.PaymentMethod,
                Note = request.Note
            };
            context.DebtPayments.Add(payment);

            LedgerEvent paymentEvent = new()
            {
                Id = Guid.NewGuid(),
                DebtId = debt.Id,
                ActorId = request.UserId,
                EventType = LedgerEventType.Payment,
                PaymentId = payment.Id,
                InternalReference = LedgerEvent.GenReferenceNumber
            };
            context.LedgerEvents.Add(paymentEvent);

            DebtPaymentStatusChange statusChange = new()
            {
                Id = Guid.NewGuid(),
                Note = "Init status",
                PaymentId = payment.Id,
                Status = PaymentStatus.Pending
            };
            context.DebtPaymentStatusChanges.Add(statusChange);

            LedgerEvent statusChangeEvent = new()
            {
                Id = Guid.NewGuid(),
                DebtId = debt.Id,
                ActorId = request.UserId,
                EventType = LedgerEventType.PaymentStatusChange,
                PaymentStatusChangeId = statusChange.Id,
                InternalReference = LedgerEvent.GenReferenceNumber
            };
            context.LedgerEvents.Add(statusChangeEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return payment.Id;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Create payment error for {UserId}", request.UserId);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
