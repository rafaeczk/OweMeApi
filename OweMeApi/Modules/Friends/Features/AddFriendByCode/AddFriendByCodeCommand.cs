using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Friends.Features.AddFriendByCode;

public record AddFriendByCodeCommand(string Code) : IRequest<HandlerResult<AddFriendResponseDTO>>;
