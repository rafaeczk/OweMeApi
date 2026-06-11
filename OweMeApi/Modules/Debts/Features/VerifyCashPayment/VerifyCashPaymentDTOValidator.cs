using FluentValidation;
using OweMeApi.Modules.Debts.Domain.Enums;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public class VerifyCashPaymentDTOValidator : AbstractValidator<VerifyCashPaymentDTO>
{
    public VerifyCashPaymentDTOValidator()
    {
        RuleFor(d => d.PaymentId)
            .NotEmpty().WithMessage("Payment id is required");

        RuleFor(d => d.Status)
            .NotEmpty().WithMessage("Payment status is required")
            .Must(DebtPaymentStatus.Verify).WithMessage($"Expected values: {string.Join(", ", DebtPaymentStatus.ValueList)}");
    }
}
