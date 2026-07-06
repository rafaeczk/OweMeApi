using FluentValidation;

namespace Application.Modules.Debts.CreateDebt;

public class CreateDebtCommandValidator : AbstractValidator<CreateDebtCommand>
{
    public CreateDebtCommandValidator()
    {
        RuleFor(d => d.DebtorId)
            .NotEmpty().WithMessage("Debtor id is required");

        RuleFor(d => d.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0m).WithMessage("Amount has to be greater than zero")
            .PrecisionScale(18, 2, false).WithMessage("Amount can have a maximum of 2 decimal places");

        RuleFor(d => d.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}
