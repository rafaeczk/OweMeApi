using FluentValidation;

namespace Application.Modules.Friends._Shared;

public class FriendRequestActionDTOValidator : AbstractValidator<FriendRequestActionDTO>
{
    public FriendRequestActionDTOValidator()
    {
        RuleFor(d => d.FriendId)
            .NotEmpty().WithMessage("Friend id is required");
    }
}
