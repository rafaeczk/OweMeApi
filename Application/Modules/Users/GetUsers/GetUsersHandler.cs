using Domain.Common;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.DTOs;
using Application.Common.Pagination;

namespace Application.Modules.Users.GetUsers;

public record GetUsersQuery(PaginationParams Pagination) : PaginationParams(Pagination), IRequest<Result<PagedResult<UserDTO>>>;

public class GetUsersHandler(IIdentityService identityService) : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDTO>>>
{
    public async Task<Result<PagedResult<UserDTO>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        return await identityService.GetUsersAsync(request);
    }
}
