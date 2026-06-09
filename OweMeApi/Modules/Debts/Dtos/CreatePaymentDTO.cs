using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Dtos;

public record CreatePaymentDTO(Guid DebtId, decimal Amount, string? Note, PaymentMethod PaymentMethod);
