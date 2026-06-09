using MediatR;
using OweMeApi.Common;
using OweMeApi.Contexts;

namespace OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

public class RequestFriendByUserIdHandler(
    FriendsService service,
    IUserContext user) : IRequestHandler<RequestFriendByUserIdCommand, HandlerResult<AddFriendResponseDTO>>
{
    public async Task<HandlerResult<AddFriendResponseDTO>> Handle(RequestFriendByUserIdCommand request, CancellationToken ct)
    {
        await service.CreateFriendshipRequest(user.Id, request.FriendId);

        return new AddFriendResponseDTO(request.FriendId);
    }
}
