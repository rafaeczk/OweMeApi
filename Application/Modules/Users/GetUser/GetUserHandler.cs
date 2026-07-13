using Application.Common;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Users.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<HandlerResult<UserDTO>>;

public class GetUserHandler(IIdentityService identityService) : IRequestHandler<GetUserQuery, HandlerResult<UserDTO>>
{
    public async Task<HandlerResult<UserDTO>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var result = await identityService.GetUser(request.UserId);

        if(!result.IsSuccess)
            return HandlerResult.Failure(result.Errors, ErrorCode.BadRequest);

        return result.Value!;
    }
}
