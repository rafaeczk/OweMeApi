using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.GetFriendRequestsList;

public record GetFriendRequestsListQuery(Guid UserId) : IRequest<HandlerResult<List<FriendRequestDTO>>>;
