namespace Application.Modules.Debts.VerifyPayment;

public record VerifyPaymentDTO(Guid PaymentId, string Status, string? Note);
