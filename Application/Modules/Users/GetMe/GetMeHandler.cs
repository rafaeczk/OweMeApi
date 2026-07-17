using Domain.Common;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Users.GetMe;

public record GetMeQuery : IRequest<Result<UserDTO>>;

public class GetMeHandler(
    IIdentityService identityService,
    IUserContext userContext) : IRequestHandler<GetMeQuery, Result<UserDTO>>
{
    public async Task<Result<UserDTO>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var result = await identityService.GetUser(userContext.Id);

        if (!result.IsSuccess)
            return Result.Failure(result.Errors, FailureReason.Unauthorized);

        return result.Value!;
    }
}
