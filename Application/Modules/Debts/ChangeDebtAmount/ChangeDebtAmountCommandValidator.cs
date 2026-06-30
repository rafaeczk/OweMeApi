using FluentValidation;

namespace Application.Modules.Debts.ChangeDebtAmount;

public class ChangeDebtAmountCommandValidator : AbstractValidator<ChangeDebtAmountCommand>
{
    public ChangeDebtAmountCommandValidator()
    {
        RuleFor(d => d.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0m).WithMessage("Amount has to be greater than zero")
            .PrecisionScale(18, 2, false).WithMessage("Amount can have a maximum of 2 decimal places");

        RuleFor(d => d.Note)
            .NotEmpty().WithMessage("Note is required");

        RuleFor(d => d.DebtId)
            .NotEmpty().WithMessage("Debt id is required");
    }
}
