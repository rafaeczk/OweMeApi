using Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Microsoft.Extensions.Logging;
using Domain.Enums;

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
            .Include(p => p.StatusChanges)
            .SingleOrDefaultAsync(p => p.Id == request.PaymentId, ct);

        if (payment == null)
            return HandlerResult.Failure("Payment not found", ErrorCode.NotFound);

        var currentPaymentStatus = payment.StatusChanges
            .Where(sc => sc.PaymentId == payment.Id)
            .OrderByDescending(sc => sc.LedgerEvent.CreatedAt)
            .Select(sc => sc.Status)
            .FirstOrDefault();

        if (currentPaymentStatus != DebtPaymentStatus.Pending)
            return HandlerResult.Failure("You have already verified this cash payment", ErrorCode.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            payment.CreateStatusChange(request.Status, request.Note);

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
