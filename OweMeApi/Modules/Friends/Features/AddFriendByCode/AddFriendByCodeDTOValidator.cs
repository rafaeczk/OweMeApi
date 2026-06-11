using FluentValidation;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public class AddFriendByCodeDTOValidator : AbstractValidator<AddFriendByCodeDTO>
{
    public AddFriendByCodeDTOValidator()
    {
        RuleFor(d => d.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6).WithMessage("Code has to be exacly 6 characters long");
    }
}
