namespace OweMeApi.Modules.Debts.Features.CreateDebt;

public record CreateDebtDTO(string Title, string? Description, Guid DebtorId, decimal Amount);
