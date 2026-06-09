using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.GetFriendsList;

public record GetFriendsListQuery(Guid UserId) : IRequest<HandlerResult<List<FriendListItemDTO>>>;
