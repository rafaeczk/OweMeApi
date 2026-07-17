using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.CreatePayment;

public record CreatePaymentCommand(Guid DebtId, decimal Amount, string? Note, string PaymentMethod) : IRequest<Result<Guid>>;

public class CreatePaymentHandler(
    IAppDbContext context,
    IUserContext user,
    ILogger<CreatePaymentHandler> logger) : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtOwnerOnly(user)
            .Where(d => d.Id == request.DebtId)
            .Include(d => d.LedgerEvents)
            .SingleOrDefaultAsync(ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var payment = debt.CreatePayment(
                new Money(request.Amount),
                user.Id,
                (debt.CreditorId == user.Id) ? debt.DebtorId : debt.CreditorId,
                request.PaymentMethod,
                request.Note);

            var statusChange = payment.CreateStatusChange(DebtPaymentStatus.Pending, "Init status");

            context.DebtPayments.Add(payment);
            context.LedgerEvents.Add(payment.LedgerEvent);
            context.DebtPaymentStatusChanges.Add(statusChange);
            context.LedgerEvents.Add(statusChange.LedgerEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return payment.Id;
        }
        catch (DebtIsSettledException exception)
        {
            logger.LogError(exception, "Change payment error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Debt is settled", FailureReason.BadRequest);
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch { }

            logger.LogError(exception, "Create payment error for: UserId={UserId}, DebtId={DebtId}", user.Id, debt.Id);

            return Result.Failure("Technical error", FailureReason.InternalError);
        }
    }
}
