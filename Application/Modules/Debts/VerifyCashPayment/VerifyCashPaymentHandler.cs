using Application.Common;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.VerifyCashPayment;

public record VerifyCashPaymentCommand(Guid PaymentId, string Status, string? Note) : IRequest<HandlerResult>;

public class VerifyCashPaymentHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<VerifyCashPaymentHandler> logger) : IRequestHandler<VerifyCashPaymentCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(VerifyCashPaymentCommand request, CancellationToken ct)
    {
        var payment = await context.DebtPayments
            .DebtPaymentReceiverOnly(user)
            .Include(p => p.LedgerEvent)
                .ThenInclude(e => e.Debt)
            .SingleOrDefaultAsync(p => p.Id == request.PaymentId, ct);

        if (payment == null)
            return HandlerResult.Failure("Payment not found", ErrorCode.NotFound);

        var currentPaymentStatus = await context.DebtPaymentStatusChanges
            .Where(sc => sc.PaymentId == payment.Id)
            .OrderByDescending(sc => sc.LedgerEvent.CreatedAt)
            .Select(sc => sc.Status)
            .FirstOrDefaultAsync(ct);

        if (currentPaymentStatus != DebtPaymentStatus.Pending)
            return HandlerResult.Failure("You have already verified this cash payment", ErrorCode.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var statusChange = payment.CreateStatusChange(request.Status, request.Note);

            context.DebtPaymentStatusChanges.Add(statusChange);
            context.LedgerEvents.Add(statusChange.LedgerEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return HandlerResult.Success();
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch { }

            logger.LogError(exception, "Payment verification error for: UserId={UserId}, PaymentId={PaymentId}", user.Id, payment.Id);

            return HandlerResult.Failure("Technical error", ErrorCode.InternalError);
        }
    }
}
