using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Users.Dtos;

namespace OweMeApi.Modules.Users.Features.GetUsers;

public record GetUsersQuery : IRequest<Result<List<UserDTO>>>;
