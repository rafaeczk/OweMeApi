using FluentValidation;

namespace Application.Modules.Users.ChangeUserPassword;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(d => d.UserId)
            .NotEmpty().WithMessage("User id is required");

        RuleFor(d => d.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
