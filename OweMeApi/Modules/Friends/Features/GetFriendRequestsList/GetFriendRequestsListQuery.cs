using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.GetFriendRequestsList;

public record GetFriendRequestsListQuery(Guid UserId) : IRequest<HandlerResult<List<FriendRequestDTO>>>;
