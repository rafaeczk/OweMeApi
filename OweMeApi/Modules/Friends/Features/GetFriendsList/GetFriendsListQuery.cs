using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.GetFriendsList;

public record GetFriendsListQuery(Guid UserId) : IRequest<HandlerResult<List<FriendListItemDTO>>>;
