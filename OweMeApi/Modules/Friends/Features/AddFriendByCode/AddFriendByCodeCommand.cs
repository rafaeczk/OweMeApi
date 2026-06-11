using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Friends.Features._Shared;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public record AddFriendByCodeCommand(string Code) : IRequest<HandlerResult<AddFriendResponseDTO>>;
