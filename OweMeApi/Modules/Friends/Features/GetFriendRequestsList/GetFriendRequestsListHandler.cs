using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Contexts.IUserContext;
using OweMeApi.Data;

namespace OweMeApi.Modules.Friends.Features.GetFriendRequestsList;

public class GetFriendRequestsListHandler(
    AppDbContext context,
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
