using Application.Common.Interfaces;
using Application.Modules.Debts._Filters;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Debts.GetDebtSummary;

public record GetDebtSummaryQuery(Guid DebtId) : IRequest<Result<DebtSummaryDTO>>;

public class GetDebtSummaryHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<GetDebtSummaryQuery, Result<DebtSummaryDTO>>
{
    public async Task<Result<DebtSummaryDTO>> Handle(GetDebtSummaryQuery query, CancellationToken ct)
    {
        var debt = await context.Debts
            .DebtParticipantOnly(user)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Payment)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.Adjustment)
            .Include(d => d.LedgerEvents)
                .ThenInclude(e => e.PaymentStatusChange)
            .SingleOrDefaultAsync(d => d.Id == query.DebtId, ct);

        if (debt is null)
            return Result.Failure("Debt not found", FailureReason.NotFound);

        var remainingToPayOff = debt.GetTotalAmount() - debt.GetTotalPayments();

        if (user.Id == debt.CreditorId)
            return remainingToPayOff > 0
                ? new DebtSummaryDTO(0, remainingToPayOff)
                : new DebtSummaryDTO(Math.Abs(remainingToPayOff), 0);

        if (user.Id == debt.DebtorId)
            return remainingToPayOff > 0
                ? new DebtSummaryDTO(remainingToPayOff, 0)
                : new DebtSummaryDTO(0, Math.Abs(remainingToPayOff));

        return new DebtSummaryDTO(0, 0);
    }
}
