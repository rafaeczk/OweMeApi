using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.RequestFriendByUserId;

public record RequestFriendByUserIdCommand(Guid UserId, Guid FriendId) : IRequest<HandlerResult<AddFriendResponseDTO>>;
