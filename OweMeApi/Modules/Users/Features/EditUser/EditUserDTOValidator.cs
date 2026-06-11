using FluentValidation;

namespace OweMeApi.Modules.Users.Features.EditUser;

public class EditUserDTOValidator : AbstractValidator<EditUserDTO>
{
    public EditUserDTOValidator()
    {
        RuleFor(d => d.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(d => d.RoleCode)
            .NotEmpty().WithMessage("Role code is required")
            .Must(v => v switch
            {
                "ADMIN" => true,
                "MODERATOR" => true,
                "USER" => true,
                "LOCKED" => true,
                _ => false
            }).WithMessage("Role code must be an exact role code");

        RuleFor(d => d.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(3).WithMessage("Min length: 3 characters");
    }
}
