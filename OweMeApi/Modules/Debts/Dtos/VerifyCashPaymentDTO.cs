using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Dtos;

public record VerifyCashPaymentDTO(Guid LedgerEventId, PaymentStatus Status, string? Note);
