using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Features._Shared;

namespace OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

public record RequestFriendByUserIdCommand(Guid FriendId) : IRequest<HandlerResult<AddFriendResponseDTO>>;
