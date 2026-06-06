namespace OweMeApi.Modules.Debts.Dtos;

public record DebtSummaryDTO(Guid PayerId, Guid ReceiverId, decimal Amount);
