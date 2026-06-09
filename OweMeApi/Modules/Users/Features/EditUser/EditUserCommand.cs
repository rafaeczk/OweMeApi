using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Users.Features.EditUser;

public record EditUserCommand(Guid UserId, string Email, string Fullname, string RoleCode) : IRequest<HandlerResult>;
