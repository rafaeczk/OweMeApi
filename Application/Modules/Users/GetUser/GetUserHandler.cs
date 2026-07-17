using Domain.Common;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Modules.Users.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<Result<UserDTO>>;

public class GetUserHandler(IIdentityService identityService) : IRequestHandler<GetUserQuery, Result<UserDTO>>
{
    public async Task<Result<UserDTO>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var result = await identityService.GetUser(request.UserId);

        if(!result.IsSuccess)
            return Result.Failure(result.Errors, FailureReason.BadRequest);

        return result.Value!;
    }
}
