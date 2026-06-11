using FluentValidation;

namespace OweMeApi.Modules.Auth.Features.SignIn;

public class SignInDTOValidator : AbstractValidator<SignInDTO>
{
    public SignInDTOValidator()
    {
        RuleFor(d => d.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(d => d.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
