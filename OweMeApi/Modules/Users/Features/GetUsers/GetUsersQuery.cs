using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Users.Features.GetUsers;

public record GetUsersQuery : IRequest<HandlerResult<List<UserDTO>>>;
