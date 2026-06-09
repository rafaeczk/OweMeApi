using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.FriendCodes.Features.GenerateMyCode;

public record GenerateMyCodeCommand(Guid UserId) : IRequest<HandlerResult<FriendCodeDTO>>;
