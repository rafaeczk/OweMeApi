using Domain.Common;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.VerifyPayment;

public record VerifyPaymentCommand(Guid PaymentId, string Status, string? Note) : IRequest<Result>;

public class VerifyPaymentHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<VerifyPaymentHandler> logger) : IRequestHandler<VerifyPaymentCommand, Result>
{
    public async Task<Result> Handle(VerifyPaymentCommand request, CancellationToken ct)
    {
        var payment = await context.DebtPayments
            .DebtPaymentReceiverOnly(user)
            .Include(p => p.LedgerEvent)
                .ThenInclude(e => e.Debt)
            .SingleOrDefaultAsync(p => p.Id == request.PaymentId, ct);

        if (payment is null)
            return Result.Failure("Payment not found", FailureReason.NotFound);

        var currentPaymentStatus = await context.DebtPaymentStatusChanges
            .Where(sc => sc.PaymentId == payment.Id)
            .OrderByDescending(sc => sc.LedgerEvent.CreatedAt)
            .Select(sc => sc.Status)
            .FirstOrDefaultAsync(ct);

        if (currentPaymentStatus != DebtPaymentStatus.Pending)
            return Result.Failure("You have already verified this payment", FailureReason.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var statusChange = payment.CreateStatusChange(request.Status, request.Note);

            context.DebtPaymentStatusChanges.Add(statusChange);
            context.LedgerEvents.Add(statusChange.LedgerEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success();
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch { }

            logger.LogError(exception, "Payment verification error for: UserId={UserId}, PaymentId={PaymentId}", user.Id, payment.Id);

            return Result.Failure("Technical error", FailureReason.InternalError);
        }
    }
}
