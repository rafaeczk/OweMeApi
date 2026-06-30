using FluentValidation;

namespace Application.Modules.Auth.SignUp;

public class SignUpDTOValidator : AbstractValidator<SignUpDTO>
{
    public SignUpDTOValidator()
    {
        RuleFor(d => d.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(d => d.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Min length: 8 characters");

        RuleFor(d => d.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(3).WithMessage("Min length: 3 characters");
    }
}
