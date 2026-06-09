using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public record VerifyCashPaymentDTO(Guid LedgerEventId, PaymentStatus Status, string? Note);
