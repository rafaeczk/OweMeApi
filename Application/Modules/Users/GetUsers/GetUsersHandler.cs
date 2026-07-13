using Application.Common;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.DTOs;
using Application.Common.Pagination;

namespace Application.Modules.Users.GetUsers;

public record GetUsersQuery(PaginationParams Pagination) : PaginationParams(Pagination), IRequest<HandlerResult<PagedResult<UserDTO>>>;

public class GetUsersHandler(IIdentityService identityService) : IRequestHandler<GetUsersQuery, HandlerResult<PagedResult<UserDTO>>>
{
    public async Task<HandlerResult<PagedResult<UserDTO>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        return await identityService.GetUsersAsync(request);
    }
}
