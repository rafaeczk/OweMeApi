using FluentValidation;

namespace OweMeApi.Modules.Debts.Features.EditDebtInformation;

public class EditDebtInformationDTOValidator : AbstractValidator<EditDebtInformationDTO>
{
    public EditDebtInformationDTOValidator()
    {
        RuleFor(d => d.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}
