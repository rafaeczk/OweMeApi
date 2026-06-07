using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.AcceptFriendRequest;

public class AcceptFriendRequestHandler(AppDbContext context)
    : IRequestHandler<AcceptFriendRequestCommand, Result>
{
    public async Task<Result> Handle(AcceptFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId && fs.FriendId == request.UserId, ct);

        if (friendship == null)
            return Result.Failure("Friendship not found", ErrorCode.NotFound);

        friendship.IsAccepted = true;
        friendship.AcceptedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
