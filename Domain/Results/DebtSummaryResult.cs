using Domain.ValueObjects;

namespace Domain.Results;

public record DebtSummaryResult(Guid PayerId, Guid ReceiverId, Money Money);
