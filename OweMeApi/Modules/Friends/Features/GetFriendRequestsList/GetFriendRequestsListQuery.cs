using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.GetFriendRequestsList;

public record GetFriendRequestsListQuery : IRequest<HandlerResult<List<FriendRequestDTO>>>;
