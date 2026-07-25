using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Debts.GetDebtActions;

public record GetDebtActionsQuery(Guid DebtId) : IRequest<Result<DebtActionsDTO>>;

public class GetDebtActionsHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtActionsQuery, Result<DebtActionsDTO>>
{
    public async Task<Result<DebtActionsDTO>> Handle(GetDebtActionsQuery query, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtParticipantOnly(user)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Payment)
                    .ThenInclude(p => p!.StatusChanges)
            .SingleOrDefaultAsync(d => d.Id == query.DebtId, ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

        var approvementEnabled = debt.GetUserCanChangeApprovement(user.Id);
        var approvementChange = new DebtApprovementChangeActionDTO(
            approvementEnabled,
            !debt.GetParticipantApproves(user.Id));

        var amountChangeEnabled = debt.GetUserCanChangeAmount(user.Id);
        var amountChange = new DebtAmountChangeActionDTO(amountChangeEnabled);

        var paymentEnabled = debt.GetUserCanCreatePayment(user.Id);
        var payment = new DebtPaymentActionDTO(paymentEnabled);

        var informationChangeEnabled = debt.GetUserCanChangeInformation(user.Id);
        var informationChange = new DebtInformationChangeActionDTO(informationChangeEnabled);

        return new DebtActionsDTO(approvementChange, amountChange, payment, informationChange);
    }
}
