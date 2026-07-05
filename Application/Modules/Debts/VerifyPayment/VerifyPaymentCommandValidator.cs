using Domain.Enums;
using FluentValidation;

namespace Application.Modules.Debts.VerifyPayment;

public class VerifyPaymentCommandValidator : AbstractValidator<VerifyPaymentCommand>
{
    public VerifyPaymentCommandValidator()
    {
        RuleFor(d => d.PaymentId)
            .NotEmpty().WithMessage("Payment id is required");

        RuleFor(d => d.Status)
            .NotEmpty().WithMessage("Payment status is required")
            .Must(DebtPaymentStatus.Verify).WithMessage($"Expected values: {string.Join(", ", DebtPaymentStatus.ValueList)}");
    }
}
