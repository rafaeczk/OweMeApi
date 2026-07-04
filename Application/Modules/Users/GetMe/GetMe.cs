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
        var result = await identityService.GetUser(userContext.Id);

        if (!result.IsSuccess)
            return HandlerResult.Failure(result.Errors, ErrorCode.Unauthorized);

        return result.Value!;
    }
}
