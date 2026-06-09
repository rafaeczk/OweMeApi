using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.GetFriendsList;

public class GetFriendsListHandler(AppDbContext context)
    : IRequestHandler<GetFriendsListQuery, HandlerResult<List<FriendListItemDTO>>>
{
    public async Task<HandlerResult<List<FriendListItemDTO>>> Handle(GetFriendsListQuery request, CancellationToken ct)
    {
        return await context.Friendships
            .Where(fs => (request.UserId == fs.UserId || request.UserId == fs.FriendId) && fs.IsAccepted)
            .Select(fs => new FriendListItemDTO(
                fs.UserId == request.UserId ? fs.Friend.Id : fs.User.Id,
                fs.UserId == request.UserId ? fs.Friend.Email : fs.User.Email,
                fs.UserId == request.UserId ? fs.Friend.FullName : fs.User.FullName,
                fs.AcceptedAt ?? fs.CreatedAt
            ))
            .ToListAsync(ct);
    }
}
