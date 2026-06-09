using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public record AddFriendByCodeCommand(Guid UserId, string Code) : IRequest<HandlerResult<AddFriendResponseDTO>>;
