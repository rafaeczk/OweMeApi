using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : IIdentityService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;

    public async Task<bool> UserExists(Guid userId)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<List<UserDTO>> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDTO>();

        foreach (var user in users)
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

    public async Task<Result<UserDTO>> GetUser(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result.Failure("User not found", FailureReason.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        if (role == null)
            return Result.Failure("Role not found", FailureReason.NotFound);

        return new UserDTO(
            user.Id,
            user.Email!,
            user.FullName,
            new UserRoleDTO(role));
    }

    public async Task<Result> SignIn(string email, string password)
    {
        var user = await _userManager.FindByNameAsync(email);

        if (user == null) 
            return Result.Failure("User not found", FailureReason.NotFound);

        var result = await _signInManager.PasswordSignInAsync(user, password, true, false);

        if (!result.Succeeded)
        {
            List<string> errors = [];

            if (result.IsLockedOut)
                errors.Add("User is locked");

            if (result.IsNotAllowed)
                errors.Add("User is not allowed");

            if (result.RequiresTwoFactor)
                errors.Add("User requires two factor");

            return Result.Failure(errors);
        }

        return Result.Success();
    }

    public async Task<Result<User>> SignUp(string email, string password, string fullName)
    {
        var user = new User()
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        var roleResult = await _userManager.AddToRoleAsync(user, UserRole.User);

        if (!roleResult.Succeeded)
            return Result.Failure(roleResult.Errors.Select(e => e.Description));

        return user;
    }

    public async Task LogOut()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<Result> ResetPassword(Guid userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null) 
            return Result.Failure("User not found", FailureReason.NotFound);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if(!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        return Result.Success();
    }
}
