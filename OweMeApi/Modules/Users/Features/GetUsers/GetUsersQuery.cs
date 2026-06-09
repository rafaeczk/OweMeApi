using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Users.Dtos;

namespace OweMeApi.Modules.Users.Features.GetUsers;

public record GetUsersQuery : IRequest<HandlerResult<List<UserDTO>>>;
