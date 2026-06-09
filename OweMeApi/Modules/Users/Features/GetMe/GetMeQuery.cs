using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Users.Features.GetMe;

public record GetMeQuery(Guid UserId) : IRequest<HandlerResult<UserDTO>>;
