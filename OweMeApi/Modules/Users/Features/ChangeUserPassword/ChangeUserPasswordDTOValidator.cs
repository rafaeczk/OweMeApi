using FluentValidation;

namespace OweMeApi.Modules.Users.Features.ChangeUserPassword;

public class ChangeUserPasswordDTOValidator : AbstractValidator<ChangeUserPasswordDTO>
{
    public ChangeUserPasswordDTOValidator()
    {
        RuleFor(d => d.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
