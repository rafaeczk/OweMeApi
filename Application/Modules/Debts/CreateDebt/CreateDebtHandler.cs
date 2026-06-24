using Application.Common;
using MediatR;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Modules.Debts.CreateDebt;

public record CreateDebtCommand(Guid DebtorId, string Title, string? Description, decimal Amount) : IRequest<HandlerResult<Guid>>;

public class CreateDebtHandler(
    IAppDbContext context,
    IIdentityService identityService,
    ILogger<CreateDebtHandler> logger,
    IUserContext user) : IRequestHandler<CreateDebtCommand, HandlerResult<Guid>>
{
    public async Task<HandlerResult<Guid>> Handle(CreateDebtCommand request, CancellationToken ct)
    {
        if (identityService.GetUserEmail(request.DebtorId) == null)
            return HandlerResult.Failure("Debtor not found", ErrorCode.NotFound);

        if (user.Id == request.DebtorId)
            return HandlerResult.Failure("You cannot debt yourself", ErrorCode.BadRequest);

        using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var debt = Debt.Create(request.Title, request.Description, user.Id, request.DebtorId);

            debt.CreateAdjustment(new Money(request.Amount), "Initial debt amount");

            context.Debts.Add(debt);

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
