using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Users.Features.EditUser;

public record EditUserCommand(Guid UserId, string Email, string FullName, string RoleCode) : IRequest<HandlerResult>;
