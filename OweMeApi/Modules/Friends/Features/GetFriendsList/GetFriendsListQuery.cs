using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.GetFriendsList;

public record GetFriendsListQuery : IRequest<HandlerResult<List<FriendListItemDTO>>>;
