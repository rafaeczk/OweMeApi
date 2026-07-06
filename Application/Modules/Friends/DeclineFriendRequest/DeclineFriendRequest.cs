using Application.Common;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;

namespace Application.Modules.Friends.DeclineFriendRequest;

public record DeclineFriendRequestCommand(Guid FriendId) : IRequest<HandlerResult>;

public class DeclineFriendRequestHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<DeclineFriendRequestCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeclineFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId, ct);

        if (friendship == null)
            return HandlerResult.Failure("Friendship not found", ErrorCode.NotFound);

        try
        {
            if (friendship.EnsureCanBeDeleted(user.Id))
            {
                context.Friendships.Remove(friendship);
                await context.SaveChangesAsync(ct);
            }

            return HandlerResult.Success();
        }
        catch (SelfFriendshipOperationException e)
        {
            return HandlerResult.Failure(e.Message, ErrorCode.Conflict);
        }
    }
}
