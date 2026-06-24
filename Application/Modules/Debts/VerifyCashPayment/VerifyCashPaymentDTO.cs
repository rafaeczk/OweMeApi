namespace Application.Modules.Debts.VerifyCashPayment;

public record VerifyCashPaymentDTO(Guid PaymentId, string Status, string? Note);
