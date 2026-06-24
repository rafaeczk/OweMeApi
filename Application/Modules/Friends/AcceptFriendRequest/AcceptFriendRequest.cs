using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common;
using Domain.Exceptions;

namespace Application.Modules.Friends.AcceptFriendRequest;

public record AcceptFriendRequestCommand(Guid FriendId) : IRequest<HandlerResult>;

public class AcceptFriendRequestHandler(
    IAppDbContext context,
    IUserContext user)
    : IRequestHandler<AcceptFriendRequestCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AcceptFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId, ct);

        if (friendship == null)
            return HandlerResult.Failure("Friendship not found", ErrorCode.NotFound);

        try
        {
            friendship.Accept(user.Id);
            await context.SaveChangesAsync(ct);

            return HandlerResult.Success();
        }
        catch(SelfFriendshipOperationException e)
        {
            return HandlerResult.Failure(e.Message, ErrorCode.Conflict);
        }
    }
}
