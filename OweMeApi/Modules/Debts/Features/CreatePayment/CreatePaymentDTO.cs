using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.CreatePayment;

public record CreatePaymentDTO(Guid DebtId, decimal Amount, string? Note, PaymentMethod PaymentMethod);
