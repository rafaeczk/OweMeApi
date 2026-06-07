using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.DeclineFriendRequest;

public class DeclineFriendRequestHandler(AppDbContext context) : IRequestHandler<DeclineFriendRequestCommand, Result>
{
    public async Task<Result> Handle(DeclineFriendRequestCommand request, CancellationToken ct)
    {
        var friendship = await context.Friendships.FirstOrDefaultAsync(fs => fs.UserId == request.FriendId && fs.FriendId == request.UserId && !fs.IsAccepted, ct);

        if (friendship == null)
            return Result.Failure("Friend request not found", ErrorCode.NotFound);

        context.Friendships.Remove(friendship);
        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
