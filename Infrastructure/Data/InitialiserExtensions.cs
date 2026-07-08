using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabase(this WebApplication app, bool dev)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.Migrate();

        await initialiser.SeedRoles();

        if (dev)
        {
            await initialiser.SeedMockUsers();
            await initialiser.SeedMockFriendships();
        }
    }
}

internal class ApplicationDbContextInitialiser(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

    public async Task Migrate()
    {
        await _context.Database.MigrateAsync();
    }

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

        User admin = new()
        {
            Id = Guid.NewGuid(),
            UserName = "a@gmail.com",
            Email = "a@gmail.com",
            FullName = "a"
        };
        var adminResult = await _userManager.CreateAsync(admin, "!23Haslo");
        if (adminResult.Succeeded)
            await _userManager.AddToRoleAsync(admin, UserRole.Admin);
        else
            throw new Exception($"Failed to create admin user: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");

        User u1 = new()
        {
            Id = Guid.NewGuid(),
            UserName = "u1@gmail.com",
            Email = "u1@gmail.com",
            FullName = "u1"
        };
        var u1Result = await _userManager.CreateAsync(u1, "!23Haslo");
        if (u1Result.Succeeded)
            await _userManager.AddToRoleAsync(u1, UserRole.User);
        else
            throw new Exception($"Failed to create u1 user: {string.Join(", ", u1Result.Errors.Select(e => e.Description))}");

        User u2 = new()
        {
            Id = Guid.NewGuid(),
            UserName = "u2@gmail.com",
            Email = "u2@gmail.com",
            FullName = "u2"
        };
        var u2Result = await _userManager.CreateAsync(u2, "!23Haslo");
        if (u2Result.Succeeded)
            await _userManager.AddToRoleAsync(u2, UserRole.User);
        else
            throw new Exception($"Failed to create u2 user: {string.Join(", ", u2Result.Errors.Select(e => e.Description))}");

        User u3 = new()
        {
            Id = Guid.NewGuid(),
            UserName = "u3@gmail.com",
            Email = "u3@gmail.com",
            FullName = "u3"
        };
        var u3Result = await _userManager.CreateAsync(u3, "!23Haslo");
        if (u3Result.Succeeded)
            await _userManager.AddToRoleAsync(u3, UserRole.User);
        else
            throw new Exception($"Failed to create u3 user: {string.Join(", ", u3Result.Errors.Select(e => e.Description))}");
    }

    public async Task SeedMockFriendships()
    {
        if (await _context.Friendships.AnyAsync()) return;

        _context.ChangeTracker.Clear();

        var user1 = await _userManager.FindByNameAsync("u1@gmail.com");
        var user2 = await _userManager.FindByNameAsync("u2@gmail.com");
        var user3 = await _userManager.FindByNameAsync("u3@gmail.com");

        if (user1 == null || user2 == null || user3 == null)
            throw new Exception($"User not found while mocking friendships. u1: {user1?.UserName}, u2: {user2?.UserName}, u3: {user3?.UserName}");

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
