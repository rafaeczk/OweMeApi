using FluentValidation;

namespace Application.Modules.Friends.RequestFriendByUserId;

public class RequestFriendByUserIdCommandValidator : AbstractValidator<RequestFriendByUserIdCommand>
{
    public RequestFriendByUserIdCommandValidator()
    {
        RuleFor(d => d.FriendId)
            .NotEmpty().WithMessage("Friend id is required");
    }
}
