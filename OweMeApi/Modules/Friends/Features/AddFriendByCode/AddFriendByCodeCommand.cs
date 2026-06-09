using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Dtos;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public record AddFriendByCodeCommand(Guid UserId, string Code) : IRequest<HandlerResult<AddFriendResponseDTO>>;
