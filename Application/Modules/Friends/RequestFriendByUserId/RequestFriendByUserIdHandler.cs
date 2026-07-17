using Application.Common.Interfaces;
using Application.Modules.Friends._Shared;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Friends.RequestFriendByUserId;

public record RequestFriendByUserIdCommand(Guid FriendId) : IRequest<Result<AddFriendResponseDTO>>;

public class RequestFriendByUserIdHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<RequestFriendByUserIdCommand, Result<AddFriendResponseDTO>>
{
    public async Task<Result<AddFriendResponseDTO>> Handle(RequestFriendByUserIdCommand request, CancellationToken ct)
    {
        if (await context.Friendships.AnyAsync(f => (f.UserId == user.Id && f.FriendId == request.FriendId) || (f.UserId == request.FriendId && f.FriendId == user.Id), ct))
            return Result.Failure("Cannot request friendship to your friend", FailureReason.BadRequest);

        var friendship = Friendship.RequestFriendship(user.Id, request.FriendId);

        context.Friendships.Add(friendship);
        await context.SaveChangesAsync(ct);

        return new AddFriendResponseDTO(request.FriendId);
    }
}
