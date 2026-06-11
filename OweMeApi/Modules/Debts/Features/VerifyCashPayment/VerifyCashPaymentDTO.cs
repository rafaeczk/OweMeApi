namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public record VerifyCashPaymentDTO(Guid PaymentId, string Status, string? Note);
