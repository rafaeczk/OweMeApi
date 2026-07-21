using Domain.Common;
using MediatR;
using Application.Common.Interfaces;
using Application.Common.DTOs;
using Application.Common.Pagination;
using Application.Common.Ordering;

namespace Application.Modules.Users.GetUsers;

public record GetUsersQuery(PaginationParams Pagination, OrderingParams Ordering) : IRequest<Result<PagedResult<UserDTO>>>;

public class GetUsersHandler(IIdentityService identityService) : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDTO>>>
{
    public async Task<Result<PagedResult<UserDTO>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        return await identityService.GetUsersAsync(request.Pagination, request.Ordering);
    }
}
