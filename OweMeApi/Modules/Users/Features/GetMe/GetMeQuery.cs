using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Users.Dtos;

namespace OweMeApi.Modules.Users.Features.GetMe;

public record GetMeQuery(Guid UserId) : IRequest<Result<UserDTO>>;
