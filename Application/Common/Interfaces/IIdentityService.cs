using Application.Common.DTOs;
using Application.Common.Pagination;
using Domain.Common;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> UserExists(Guid userId);

    Task<Result<UserDTO>> GetUser(Guid userId);

    Task<PagedResult<UserDTO>> GetUsersAsync(PaginationParams pagination);

    Task<Result<string>> SignIn(string email, string password);

    Task<Result<User>> SignUp(string email, string password, string fullName);

    Task<Result> ResetPassword(Guid userId, string newPassword);
}
