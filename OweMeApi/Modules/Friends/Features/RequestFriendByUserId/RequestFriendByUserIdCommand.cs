using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

public record RequestFriendByUserIdCommand(Guid UserId, Guid FriendId) : IRequest<HandlerResult<AddFriendResponseDTO>>;
