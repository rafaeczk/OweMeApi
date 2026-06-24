namespace Application.Modules.Debts.CreateDebt;

public record CreateDebtDTO(string Title, string? Description, Guid DebtorId, decimal Amount);
