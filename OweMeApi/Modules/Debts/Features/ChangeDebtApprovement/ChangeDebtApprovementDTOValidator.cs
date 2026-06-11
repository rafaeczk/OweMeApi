using FluentValidation;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtApprovement;

public class ChangeDebtApprovementDTOValidator : AbstractValidator<ChangeDebtApprovementDTO>
{
    public ChangeDebtApprovementDTOValidator()
    {
        RuleFor(d => d.Approve)
            .NotEmpty().WithMessage("Approve field is required");
    }
}
