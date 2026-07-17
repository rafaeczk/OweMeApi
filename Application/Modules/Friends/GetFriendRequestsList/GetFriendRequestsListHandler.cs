using Domain.Common;
using Application.Common.Interfaces;
using Application.Common.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Friends.GetFriendRequestsList;

public record GetFriendRequestsListQuery(PaginationParams Pagination) : PaginationParams(Pagination), IRequest<Result<PagedResult<FriendRequestDTO>>>;

public class GetFriendRequestsListHandler(
    IAppDbContext context,
    IUserContext user)
    : IRequestHandler<GetFriendRequestsListQuery, Result<PagedResult<FriendRequestDTO>>>
{
    public async Task<Result<PagedResult<FriendRequestDTO>>> Handle(GetFriendRequestsListQuery request, CancellationToken ct)
    {
        var frienshipsQuery = context.Friendships;

        var totalFriendships = await frienshipsQuery.CountAsync(ct);

        var friendships = await frienshipsQuery
            .Where(fs => fs.FriendId == user.Id && !fs.IsAccepted)
            .Paginate(request, f => f.AcceptedAt ?? f.CreatedAt)
            .Select(fs => new FriendRequestDTO(
                fs.UserId == user.Id ? fs.Friend.Id : fs.User.Id,
                fs.UserId == user.Id ? fs.Friend.Email! : fs.User.Email!,
                fs.UserId == user.Id ? fs.Friend.FullName : fs.User.FullName,
                fs.CreatedAt
            ))
            .ToListAsync(ct);

        return new PagedResult<FriendRequestDTO>(friendships, totalFriendships, request);
    }
}
