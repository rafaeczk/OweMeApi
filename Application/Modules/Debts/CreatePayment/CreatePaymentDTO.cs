namespace Application.Modules.Debts.CreatePayment;

public record CreatePaymentDTO(Guid DebtId, decimal Amount, string? Note, string PaymentMethod);
