using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Users.Features.ChangeUserPassword;

public record ChangeUserPasswordCommand(Guid UserId, string Password) : IRequest<HandlerResult>;
