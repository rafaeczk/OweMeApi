using FluentValidation;

namespace OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

public class RequestFriendByUserIdDTOValidator : AbstractValidator<RequestFriendByUserIdDTO>
{
    public RequestFriendByUserIdDTOValidator()
    {
        RuleFor(d => d.FriendId)
            .NotEmpty().WithMessage("Friend id is required");
    }
}
