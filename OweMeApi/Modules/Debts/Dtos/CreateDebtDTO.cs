namespace OweMeApi.Modules.Debts.Dtos;

public record CreateDebtDTO(string Title, string? Description, Guid DebtorId, decimal Amount);
