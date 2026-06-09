using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.GetFriendRequestsList;

public class GetFriendRequestsListHandler(AppDbContext context)
    : IRequestHandler<GetFriendRequestsListQuery, HandlerResult<List<FriendRequestDTO>>>
{
    public async Task<HandlerResult<List<FriendRequestDTO>>> Handle(GetFriendRequestsListQuery request, CancellationToken ct)
    {
        return await context.Friendships
            .Where(fs => fs.FriendId == request.UserId && !fs.IsAccepted)
            .Select(fs => new FriendRequestDTO(
                fs.UserId == request.UserId ? fs.Friend.Id : fs.User.Id,
                fs.UserId == request.UserId ? fs.Friend.Email : fs.User.Email,
                fs.UserId == request.UserId ? fs.Friend.FullName : fs.User.FullName,
                fs.CreatedAt
            ))
            .ToListAsync(ct);
    }
}
