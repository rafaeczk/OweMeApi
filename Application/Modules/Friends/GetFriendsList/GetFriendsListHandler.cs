using Domain.Common;
using Application.Common.Interfaces;
using Application.Common.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Ordering;

namespace Application.Modules.Friends.GetFriendsList;

public record GetFriendsListQuery(PaginationParams Pagination, OrderingParams Ordering) : IRequest<Result<PagedResult<FriendListItemDTO>>>;

public class GetFriendsListHandler(
    IAppDbContext context,
    IUserContext user)
    : IRequestHandler<GetFriendsListQuery, Result<PagedResult<FriendListItemDTO>>>
{
    public async Task<Result<PagedResult<FriendListItemDTO>>> Handle(GetFriendsListQuery request, CancellationToken ct)
    {
        var friendshipsQuery = context.Friendships.Where(fs => (user.Id == fs.UserId || user.Id == fs.FriendId) && fs.IsAccepted);

        var totalFriendships = await friendshipsQuery.CountAsync(ct);

        var friends = await friendshipsQuery
            .Order(request.Ordering)
            .Paginate(request.Pagination)
            .Select(fs => new FriendListItemDTO(
                fs.UserId == user.Id ? fs.Friend.Id : fs.User.Id,
                fs.UserId == user.Id ? fs.Friend.Email! : fs.User.Email!,
                fs.UserId == user.Id ? fs.Friend.FullName : fs.User.FullName,
                fs.AcceptedAt ?? fs.CreatedAt
            ))
            .ToListAsync(ct);

        return new PagedResult<FriendListItemDTO>(friends, totalFriendships, request.Pagination);
    }
}
