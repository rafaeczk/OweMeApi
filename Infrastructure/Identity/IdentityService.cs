using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<IdentityRole<Guid>> roleManager) : IIdentityService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

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
                    user.Email!,
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
        User user = new()
        {
            Email = email,
            UserName = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.Succeeded, user.Id.ToString());
    }

    public async Task<bool> SignIn(string email, string password)
    {
        var user = await _userManager.FindByNameAsync(email);

        if (user == null) return false;

        var result = await _signInManager.PasswordSignInAsync(user, password, true, false);

        return result.Succeeded;
    }

    public async Task<(bool, User)> SignUp(string email, string password, string fullName)
    {
        var user = new User()
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        var roleResult = await _userManager.AddToRoleAsync(user, UserRole.User);

        return (result.Succeeded && roleResult.Succeeded, user);
    }

    public async Task LogOut()
    {
        await _signInManager.SignOutAsync();
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
