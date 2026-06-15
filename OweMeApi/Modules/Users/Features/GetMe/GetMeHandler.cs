using MediatR;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Users.Features.GetMe;

public class GetMeHandler(AppDbContext context) : IRequestHandler<GetMeQuery, HandlerResult<UserDTO>>
{
    public async Task<HandlerResult<UserDTO>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var user = await context.Users
            .FindAsync([ request.UserId, ct ], ct);

        if (user == null)
            return HandlerResult.Failure("User not found", ErrorCode.NotFound);

        return new UserDTO(
            user.Id,
            user.Email,
            user.FullName,
            new UserRoleDTO(user.RoleCode)
        );
    }
}
