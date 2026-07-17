using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Pagination;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity;

public class IdentityService(
    UserManager<User> userManager,
    IConfiguration configuration) : IIdentityService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<bool> UserExists(Guid userId)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<PagedResult<UserDTO>> GetUsersAsync(PaginationParams pagination)
    {
        var usersQuery = _userManager.Users;

        var totalUsers = await usersQuery.CountAsync();

        usersQuery = usersQuery.Paginate(pagination, u => u.Id);

        var users = await usersQuery.ToListAsync();

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

        return new(userDtos, totalUsers, pagination);
    }

    public async Task<Result<UserDTO>> GetUser(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure("User not found", FailureReason.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        if (role is null)
            return Result.Failure("Role not found", FailureReason.NotFound);

        return new UserDTO(
            user.Id,
            user.Email!,
            user.FullName,
            new UserRoleDTO(role));
    }

    public async Task<Result<string>> SignIn(string email, string password)
    {
        var user = await _userManager.FindByNameAsync(email);

        if (user is null) 
            return Result.Failure("Invalid credentials", FailureReason.Unauthorized);

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return Result.Failure("Invalid credentials", FailureReason.Unauthorized);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var secretKey = _configuration["JwtSettings:Secret"]
            ?? throw new Exception("JWT Secret missing in configuration!");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
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
            return Result.Failure(result.Errors.Select(e => e.Description), FailureReason.BadRequest);

        var roleResult = await _userManager.AddToRoleAsync(user, UserRole.User);

        if (!roleResult.Succeeded)
            return Result.Failure(roleResult.Errors.Select(e => e.Description), FailureReason.BadRequest);

        return user;
    }

    public async Task<Result> ResetPassword(Guid userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null) 
            return Result.Failure("User not found", FailureReason.NotFound);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if(!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description), FailureReason.BadRequest);

        return Result.Success();
    }
}
