using Application.Common.Interfaces;
using Application.Modules.Friends._Shared;
using MediatR;
using Application.Common;
using Domain.Entities;

namespace Application.Modules.Friends.RequestFriendByUserId;

public record RequestFriendByUserIdCommand(Guid FriendId) : IRequest<HandlerResult<AddFriendResponseDTO>>;

public class RequestFriendByUserIdHandler(
    IAppDbContext context,
    IUserContext user) : IRequestHandler<RequestFriendByUserIdCommand, HandlerResult<AddFriendResponseDTO>>
{
    public async Task<HandlerResult<AddFriendResponseDTO>> Handle(RequestFriendByUserIdCommand request, CancellationToken ct)
    {
        var friendship = Friendship.RequestFriendship(user.Id, request.FriendId);

        context.Friendships.Add(friendship);
        await context.SaveChangesAsync(ct);

        return new AddFriendResponseDTO(request.FriendId);
    }
}
