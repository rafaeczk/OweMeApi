using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;

namespace OweMeApi.Modules.Users.Features.GetUsers;

public class GetUsersHandler(AppDbContext context) : IRequestHandler<GetUsersQuery, HandlerResult<List<UserDTO>>>
{
    public async Task<HandlerResult<List<UserDTO>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await context.Users
            .Select(u =>
                new UserDTO(
                    u.Id,
                    u.Email,
                    u.FullName,
                    new UserRoleDTO(u.RoleCode)
                )
            )
            .ToListAsync(ct);

        return users;
    }
}
