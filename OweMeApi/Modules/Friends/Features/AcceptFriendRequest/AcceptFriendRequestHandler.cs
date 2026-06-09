using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.AcceptFriendRequest;

public class AcceptFriendRequestHandler(AppDbContext context)
    : IRequestHandler<AcceptFriendRequestCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AcceptFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId && fs.FriendId == request.UserId, ct);

        if (friendship == null)
            return HandlerResult.Failure("Friendship not found", ErrorCode.NotFound);

        friendship.IsAccepted = true;
        friendship.AcceptedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return HandlerResult.Success();
    }
}
