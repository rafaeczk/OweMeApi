using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.AcceptFriendRequest;

public record AcceptFriendRequestCommand(Guid UserId, Guid FriendId) : IRequest<HandlerResult>;
