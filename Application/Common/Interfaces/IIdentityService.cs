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

    Task<bool> AuthorizeAsync(Guid userId, string policyName);

    Task<bool> ResetPassword(Guid userId, string newPassword);
}
