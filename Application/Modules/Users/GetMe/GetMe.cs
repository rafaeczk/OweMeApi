using Application.Common;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Users.GetMe;

public record GetMeQuery : IRequest<HandlerResult<UserDTO>>;

public class GetMeHandler(
    IIdentityService identityService,
    IUserContext userContext) : IRequestHandler<GetMeQuery, HandlerResult<UserDTO>>
{
    public async Task<HandlerResult<UserDTO>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var (userFound, user) = await identityService.GetUserById(userContext.Id);

        if (!userFound)
            return HandlerResult.Failure("User not found", ErrorCode.NotFound);

        var (_, role) = await identityService.GetUserRole(user.Id);

        return new UserDTO(user.Id, user.Email!, user.FullName, new UserRoleDTO(role ?? "Not found"));
    }
}
