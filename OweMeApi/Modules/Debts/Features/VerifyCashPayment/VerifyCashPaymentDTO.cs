using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public record VerifyCashPaymentDTO(Guid PaymentId, PaymentStatus Status, string? Note);
