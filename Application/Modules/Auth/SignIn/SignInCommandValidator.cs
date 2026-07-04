using FluentValidation;

namespace Application.Modules.Auth.SignIn;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(d => d.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(d => d.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
