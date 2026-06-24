using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseDevMock(this WebApplication app, bool dev)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedRoles();
        if (dev)
        {
            await initialiser.SeedMockUsers();
            await initialiser.SeedMockFriendships();
        }
    }
}

public class ApplicationDbContextInitialiser(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task SeedRoles()
    {
        if (!await _roleManager.RoleExistsAsync(UserRole.Admin))
            await _roleManager.CreateAsync(new(UserRole.Admin));

        if (!await _roleManager.RoleExistsAsync(UserRole.User))
            await _roleManager.CreateAsync(new(UserRole.User));
    }

    public async Task SeedMockUsers()
    {
        if (await _userManager.Users.AnyAsync()) return;

        AppUser admin = new()
        {
            Id = Guid.NewGuid(),
            UserName = "a@gmail.com",
            Email = "a@gmail.com",
            FullName = "a"
        };
        if ((await _userManager.CreateAsync(admin, "123")).Succeeded)
            await _userManager.AddToRoleAsync(admin, UserRole.Admin);

        AppUser u1 = new()
        {
            Id = Guid.NewGuid(),
            UserName = "u1@gmail.com",
            Email = "u1@gmail.com",
            FullName = "u1"
        };
        if ((await _userManager.CreateAsync(u1, "123")).Succeeded)
            await _userManager.AddToRoleAsync(u1, UserRole.User);

        AppUser u2 = new()
        {
            Id = Guid.NewGuid(),
            UserName = "u2@gmail.com",
            Email = "u2@gmail.com",
            FullName = "u2"
        };
        if ((await _userManager.CreateAsync(u2, "123")).Succeeded)
            await _userManager.AddToRoleAsync(u2, UserRole.User);

        AppUser u3 = new()
        {
            Id = Guid.NewGuid(),
            UserName = "u3@gmail.com",
            Email = "u3@gmail.com",
            FullName = "u3"
        };
        if ((await _userManager.CreateAsync(u3, "123")).Succeeded)
            await _userManager.AddToRoleAsync(u3, UserRole.User);
    }

    public async Task SeedMockFriendships()
    {
        if (await _context.Friendships.AnyAsync()) return;

        var user1 = await _userManager.FindByEmailAsync("u1@gmail.com");
        var user2 = await _userManager.FindByEmailAsync("u2@gmail.com");
        var user3 = await _userManager.FindByEmailAsync("u3@gmail.com");

        if (user1 == null || user2 == null || user3 == null)
            throw new Exception("User not found while mocking friendships");

        var friendships = new List<Friendship>
        {
            Friendship.FromFriendCode(user1.Id, FriendCode.ForUser(user2.Id)),
            Friendship.FromFriendCode(user1.Id, FriendCode.ForUser(user3.Id)),
            Friendship.FromFriendCode(user2.Id, FriendCode.ForUser(user3.Id))
        };

        await _context.Friendships.AddRangeAsync(friendships);
        await _context.SaveChangesAsync();
    }
}
