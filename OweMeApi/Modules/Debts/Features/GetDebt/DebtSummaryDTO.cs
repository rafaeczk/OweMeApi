namespace OweMeApi.Modules.Debts.Features.GetDebt;

public record DebtSummaryDTO(Guid PayerId, Guid ReceiverId, decimal Amount);
