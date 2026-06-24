using FluentValidation;

namespace Application.Modules.Debts.EditDebtInformation;

public class EditDebtInformationDTOValidator : AbstractValidator<EditDebtInformationDTO>
{
    public EditDebtInformationDTOValidator()
    {
        RuleFor(d => d.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}
