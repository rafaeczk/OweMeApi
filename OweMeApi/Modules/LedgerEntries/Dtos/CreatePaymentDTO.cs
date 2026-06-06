using OweMeApi.Data.Entities;

namespace OweMeApi.Modules.LedgerEntries.Dtos;

public record CreatePaymentDTO(Guid DebtId, decimal Amount, string? Note, PaymentMethod PaymentMethod);
