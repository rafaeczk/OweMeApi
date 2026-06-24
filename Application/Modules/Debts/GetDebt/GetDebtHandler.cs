using Application.Common;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Modules.Debts.GetDebt;

public record GetDebtQuery(Guid DebtId) : IRequest<HandlerResult<DebtDTO>>;

public class GetDebtHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtQuery, HandlerResult<DebtDTO>>
{
    public async Task<HandlerResult<DebtDTO>> Handle(GetDebtQuery request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtOwnerOnly(user)
            .FirstOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt == null)
            return HandlerResult.Failure("Debt not found", ErrorCode.NotFound);

        var totalAmount = debt.GetTotalAmount();

        var totalPayments = debt.GetTotalPayments();

        var summary = Debt.CalcDebtSummary(totalAmount, totalPayments, debt.CreditorId, debt.DebtorId);

        var creditorApproves = debt.GetCreditorApproves();

        var debtorApproves = debt.GetDebtorApproves();

        var debtIsSettled = debt.GetIsSettled();

        return new DebtDTO(
            debt.Id,
            debt.Title,
            debt.Description,
            debt.CreditorId,
            debt.DebtorId,
            totalAmount,
            totalPayments,
            summary,
            creditorApproves,
            debtorApproves,
            debtIsSettled
        );
    }
}
