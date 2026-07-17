using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Debts.CreateDebt;

public record CreateDebtCommand(Guid DebtorId, string Title, string? Description, decimal Amount) : IRequest<Result<Guid>>;

public class CreateDebtHandler(
    IAppDbContext context,
    IIdentityService identityService,
    ILogger<CreateDebtHandler> logger,
    IUserContext user) : IRequestHandler<CreateDebtCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateDebtCommand request, CancellationToken ct)
    {
        if (!await identityService.UserExists(request.DebtorId))
            return Result.Failure("Debtor not found", FailureReason.NotFound);

        if (user.Id == request.DebtorId)
            return Result.Failure("You cannot debt yourself", FailureReason.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var debt = Debt.Create(request.Title, request.Description, user.Id, request.DebtorId);

            var adjustment = debt.CreateAdjustment(new Money(request.Amount), "Initial debt amount");

            context.Debts.Add(debt);
            context.DebtAdjustments.Add(adjustment);
            context.LedgerEvents.Add(adjustment.LedgerEvent);

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return debt.Id;
        }
        catch(Exception exception)
        {
            logger.LogError(exception, "Create debt error for {DebtorId}", request.DebtorId);

            return Result.Failure("Technical error", FailureReason.InternalError);
        }
    }
}
