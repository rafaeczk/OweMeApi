using FluentValidation;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtAmount;

public class ChangeDebtAmountDTOValidator : AbstractValidator<ChangeDebtAmountDTO>
{
    public ChangeDebtAmountDTOValidator()
    {
        RuleFor(d => d.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0m).WithMessage("Amount has to be greater than zero")
            .PrecisionScale(18, 2, false).WithMessage("Amount can have a maximum of 2 decimal places");

        RuleFor(d => d.Note)
            .NotEmpty().WithMessage("Note is required");
    }
}
