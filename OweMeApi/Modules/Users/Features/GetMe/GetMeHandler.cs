using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Modules.UserRoles.Dtos;
using OweMeApi.Modules.Users.Dtos;

namespace OweMeApi.Modules.Users.Features.GetMe;

public class GetMeHandler(AppDbContext context) : IRequestHandler<GetMeQuery, HandlerResult<UserDTO>>
{
    public async Task<HandlerResult<UserDTO>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user == null)
            return HandlerResult.Failure("User not found", ErrorCode.NotFound);

        return new UserDTO(
            user.Id,
            user.Email,
            user.FullName,
            new UserRoleDTO(user.RoleCode, user.Role!.Label)
        );
    }
}
