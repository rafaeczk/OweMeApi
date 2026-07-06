using FluentValidation;

namespace Application.Modules.Friends.AddFriendByCode;

public class AddFriendByCodeCommandValidator : AbstractValidator<AddFriendByCodeCommand>
{
    public AddFriendByCodeCommandValidator()
    {
        RuleFor(d => d.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6).WithMessage("Code has to be exacly 6 characters long");
    }
}
