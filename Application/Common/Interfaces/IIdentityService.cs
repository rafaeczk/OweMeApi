using Application.Common.DTOs;
using Domain.Common;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> UserExists(Guid userId);

    Task<Result<UserDTO>> GetUser(Guid userId);

    Task<List<UserDTO>> GetUsersAsync();

    Task<Result> SignIn(string email, string password);

    Task<Result<User>> SignUp(string email, string password, string fullName);

    Task LogOut();

    Task<Result> ResetPassword(Guid userId, string newPassword);
}
