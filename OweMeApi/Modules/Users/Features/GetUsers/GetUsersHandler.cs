using MediatR;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Common;
using OweMeApi.Data;
using OweMeApi.Modules.UserRoles.Dtos;

namespace OweMeApi.Modules.Users.Features.GetUsers;

public class GetUsersHandler(AppDbContext context) : IRequestHandler<GetUsersQuery, HandlerResult<List<UserDTO>>>
{
    public async Task<HandlerResult<List<UserDTO>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await context.Users
            .Include(u => u.Role)
            .Select(u =>
                new UserDTO(
                    u.Id,
                    u.Email,
                    u.FullName,
                    new UserRoleDTO(u.RoleCode, u.Role!.Label)
                )
            )
            .ToListAsync(ct);

        return users;
    }
}
