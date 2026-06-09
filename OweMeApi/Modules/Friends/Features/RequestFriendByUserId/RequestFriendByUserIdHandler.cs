using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

public class RequestFriendByUserIdHandler(FriendsService service) : IRequestHandler<RequestFriendByUserIdCommand, HandlerResult<AddFriendResponseDTO>>
{
    public async Task<HandlerResult<AddFriendResponseDTO>> Handle(RequestFriendByUserIdCommand request, CancellationToken ct)
    {
        await service.CreateFriendshipRequest(request.UserId, request.FriendId);

        return new AddFriendResponseDTO(request.FriendId);
    }
}
