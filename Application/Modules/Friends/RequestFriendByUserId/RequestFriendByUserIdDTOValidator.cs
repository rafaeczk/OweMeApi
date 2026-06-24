using FluentValidation;

namespace Application.Modules.Friends.RequestFriendByUserId;

public class RequestFriendByUserIdDTOValidator : AbstractValidator<RequestFriendByUserIdDTO>
{
    public RequestFriendByUserIdDTOValidator()
    {
        RuleFor(d => d.FriendId)
            .NotEmpty().WithMessage("Friend id is required");
    }
}
