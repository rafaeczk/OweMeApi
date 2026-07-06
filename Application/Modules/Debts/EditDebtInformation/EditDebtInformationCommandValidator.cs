using FluentValidation;

namespace Application.Modules.Debts.EditDebtInformation;

public class EditDebtInformationCommandValidator : AbstractValidator<EditDebtInformationCommand>
{
    public EditDebtInformationCommandValidator()
    {
        RuleFor(d => d.DebtId)
            .NotEmpty().WithMessage("Debt id is required");

        RuleFor(d => d.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}
