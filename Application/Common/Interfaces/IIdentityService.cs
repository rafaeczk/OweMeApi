using Application.Common.DTOs;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserName(Guid userId);

    Task<string?> GetUserEmail(Guid userId);

    Task<(bool, User)> GetUserById(Guid userId);

    Task<List<UserDTO>> GetUsersAsync();

    Task<(bool, string)> GetUserRole(Guid userId);

    Task<(bool, string)> CreateUser(string email, string fullName, string password);

    Task<bool> SignIn(string email, string password);

    Task<(bool, User)> SignUp(string email, string password, string fullName);

    Task LogOut();

    Task<bool> ResetPassword(Guid userId, string newPassword);
}
