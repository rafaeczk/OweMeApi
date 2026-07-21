using Domain.Common;
using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Modules.Debts.GetDebt;

public record GetDebtQuery(Guid DebtId) : IRequest<Result<DebtDTO>>;

public class GetDebtHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtQuery, Result<DebtDTO>>
{
    public async Task<Result<DebtDTO>> Handle(GetDebtQuery request, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtOwnerOnly(user)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Payment)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Adjustment)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.PaymentStatusChange)
            .SingleOrDefaultAsync(d => d.Id == request.DebtId, ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

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
            debt.CreditorId == user.Id,
            debt.DebtorId == user.Id,
            debt.CreatedAt,
            totalAmount,
            totalPayments,
            summary,
            creditorApproves,
            debtorApproves,
            debtIsSettled
        );
    }
}
