using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class IdentityService(
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<string?> GetUserName(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user?.FullName;
    }

    public async Task<string?> GetUserEmail(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user?.Email;
    }

    public async Task<(bool, User)> GetUserById(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return (false, null!);

        return (true, user);
    }

    public async Task<List<UserDTO>> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDTO>();

        foreach(var user in users)
        {
            var role = await _userManager.GetRolesAsync(user);

            userDtos.Add(new UserDTO(
                    user.Id,
                    user.Email,
                    user.FullName,
                    new UserRoleDTO(role.First())
                ));
        }

        return userDtos;
    }

    public async Task<(bool, string)> GetUserRole(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return (false, null!);

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        if (role == null)
            return (false, null!);

        return (true, role);
    }

    public async Task<(bool, string)> CreateUser(string email, string fullName, string password)
    {
        AppUser user = new()
        {
            Email = email,
            UserName = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.Succeeded, user.Id.ToString());
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<bool> ResetPassword(Guid userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null) return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded;
    }
}
