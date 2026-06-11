using FluentValidation;
using OweMeApi.Modules.Debts.Domain.Enums;

namespace OweMeApi.Modules.Debts.Features.CreatePayment;

public class CreatePaymentDTOValidator : AbstractValidator<CreatePaymentDTO>
{
    public CreatePaymentDTOValidator()
    {
        RuleFor(d => d.DebtId)
            .NotEmpty().WithMessage("Debt id is required");

        RuleFor(d => d.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0m).WithMessage("Amount has to be greater than zero")
            .PrecisionScale(18, 2, false).WithMessage("Amount can have a maximum of 2 decimal places");

        RuleFor(d => d.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required")
            .Must(DebtPaymentMethod.Verify).WithMessage($"Expected values: {string.Join(", ", DebtPaymentMethod.ValueList)}");
    }
}
