using Application.Common;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Friends.GetFriendRequestsList;

public record GetFriendRequestsListQuery : IRequest<HandlerResult<List<FriendRequestDTO>>>;

public class GetFriendRequestsListHandler(
    IAppDbContext context,
    IUserContext user)
    : IRequestHandler<GetFriendRequestsListQuery, HandlerResult<List<FriendRequestDTO>>>
{
    public async Task<HandlerResult<List<FriendRequestDTO>>> Handle(GetFriendRequestsListQuery request, CancellationToken ct)
    {
        return await context.Friendships
            .Where(fs => fs.FriendId == user.Id && !fs.IsAccepted)
            .Select(fs => new FriendRequestDTO(
                fs.UserId == user.Id ? fs.Friend.Id : fs.User.Id,
                fs.UserId == user.Id ? fs.Friend.Email : fs.User.Email,
                fs.UserId == user.Id ? fs.Friend.FullName : fs.User.FullName,
                fs.CreatedAt
            ))
            .ToListAsync(ct);
    }
}
