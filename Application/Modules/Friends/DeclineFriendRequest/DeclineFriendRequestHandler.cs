using Domain.Common;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;

namespace Application.Modules.Friends.DeclineFriendRequest;

public record DeclineFriendRequestCommand(Guid FriendId) : IRequest<Result>;

public class DeclineFriendRequestHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<DeclineFriendRequestCommand, Result>
{
    public async Task<Result> Handle(DeclineFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId, ct);

        if (friendship is null)
            return Result.Failure("Friendship not found", FailureReason.NotFound);

        try
        {
            if (friendship.EnsureCanBeDeleted(user.Id))
            {
                context.Friendships.Remove(friendship);
                await context.SaveChangesAsync(ct);
            }

            return Result.Success();
        }
        catch (SelfFriendshipOperationException e)
        {
            return Result.Failure(e.Message, FailureReason.Conflict);
        }
    }
}
