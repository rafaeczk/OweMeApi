using FluentValidation;

namespace Application.Modules.Users.ChangeUserPassword;

public class ChangeUserPasswordDTOValidator : AbstractValidator<ChangeUserPasswordDTO>
{
    public ChangeUserPasswordDTOValidator()
    {
        RuleFor(d => d.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
