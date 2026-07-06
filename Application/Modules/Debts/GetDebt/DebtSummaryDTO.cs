using Domain.Results;
using Domain.ValueObjects;

namespace Application.Modules.Debts.GetDebt;

public record DebtSummaryDTO(Guid PayerId, Guid ReceiverId, Money Money)
{
    public static implicit operator DebtSummaryDTO(DebtSummaryResult summary)
        => new(summary.PayerId, summary.ReceiverId, summary.Money);
}
