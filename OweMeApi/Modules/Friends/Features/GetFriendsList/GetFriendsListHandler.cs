using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.GetFriendsList;

public class GetFriendsListHandler(
    AppDbContext context,
    IUserContext user)
    : IRequestHandler<GetFriendsListQuery, HandlerResult<List<FriendListItemDTO>>>
{
    public async Task<HandlerResult<List<FriendListItemDTO>>> Handle(GetFriendsListQuery request, CancellationToken ct)
    {
        return await context.Friendships
            .Where(fs => (user.Id == fs.UserId || user.Id == fs.FriendId) && fs.IsAccepted)
            .Select(fs => new FriendListItemDTO(
                fs.UserId == user.Id ? fs.Friend.Id : fs.User.Id,
                fs.UserId == user.Id ? fs.Friend.Email : fs.User.Email,
                fs.UserId == user.Id ? fs.Friend.FullName : fs.User.FullName,
                fs.AcceptedAt ?? fs.CreatedAt
            ))
            .ToListAsync(ct);
    }
}
