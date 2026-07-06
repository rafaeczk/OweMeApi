using Application.Common;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.DTOs;

namespace Application.Modules.Users.GetUsers;

public record GetUsersQuery : IRequest<HandlerResult<List<UserDTO>>>;

public class GetUsersHandler(IIdentityService identityService) : IRequestHandler<GetUsersQuery, HandlerResult<List<UserDTO>>>
{
    public async Task<HandlerResult<List<UserDTO>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        return await identityService.GetUsersAsync();
    }
}
