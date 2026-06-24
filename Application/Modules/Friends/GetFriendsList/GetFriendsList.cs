using Application.Common;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Friends.GetFriendsList;

public record GetFriendsListQuery : IRequest<HandlerResult<List<FriendListItemDTO>>>;

public class GetFriendsListHandler(
    IAppDbContext context,
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
