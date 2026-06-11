using FluentValidation;

namespace OweMeApi.Modules.Friends.Features._Shared;

public class FriendRequestActionDTOValidator : AbstractValidator<FriendRequestActionDTO>
{
    public FriendRequestActionDTOValidator()
    {
        RuleFor(d => d.FriendId)
            .NotEmpty().WithMessage("Friend id is required");
    }
}
