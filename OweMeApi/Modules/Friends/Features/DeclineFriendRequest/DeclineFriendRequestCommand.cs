using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.DeclineFriendRequest;

public record DeclineFriendRequestCommand(Guid UserId, Guid FriendId) : IRequest<Result>;