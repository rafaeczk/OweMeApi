using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Domain.Exceptions;

namespace Application.Modules.Friends.AcceptFriendRequest;

public record AcceptFriendRequestCommand(Guid FriendId) : IRequest<Result>;

public class AcceptFriendRequestHandler(
    IAppDbContext context,
    IUserContext user)
    : IRequestHandler<AcceptFriendRequestCommand, Result>
{
    public async Task<Result> Handle(AcceptFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId, ct);

        if (friendship is null)
            return Result.Failure("Friendship not found", FailureReason.NotFound);

        if (friendship.IsAccepted)
            return Result.Failure("Friendship already accepted", FailureReason.BadRequest);

        try
        {
            friendship.Accept(user.Id);
            await context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch(SelfFriendshipOperationException e)
        {
            return Result.Failure(e.Message, FailureReason.Conflict);
        }
    }
}
