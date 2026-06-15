using Microsoft.EntityFrameworkCore;
using OweMeApi.Data.Entities;
using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Data;

public static class Mock
{
    public static async Task Init(IServiceProvider serviceProvider, IWebHostEnvironment env)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (env.IsProduction()) return;

        await GenUsers(context);
        await GenFriendships(context);
    }

    private static async Task GenUsers(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        User admin = new()
        {
            Id = Guid.NewGuid(),
            Email = "a@gmail.com",
            FullName = "a",
            RoleCode = UserRole.Admin,
            Hash = BCrypt.Net.BCrypt.HashPassword("123")
        };

        User user1 = new()
        {
            Id = Guid.NewGuid(),
            Email = "u1@gmail.com",
            FullName = "u1",
            RoleCode = UserRole.User,
            Hash = BCrypt.Net.BCrypt.HashPassword("123")
        };

        User user2 = new()
        {
            Id = Guid.NewGuid(),
            Email = "u2@gmail.com",
            FullName = "u2",
            RoleCode = UserRole.User,
            Hash = BCrypt.Net.BCrypt.HashPassword("123")
        };

        User user3 = new()
        {
            Id = Guid.NewGuid(),
            Email = "u3@gmail.com",
            FullName = "u3",
            RoleCode = UserRole.User,
            Hash = BCrypt.Net.BCrypt.HashPassword("123")
        };

        context.Users.Add(admin);
        context.Users.Add(user1);
        context.Users.Add(user2);
        context.Users.Add(user3);
        await context.SaveChangesAsync();
    }

    private static async Task GenFriendships(AppDbContext context)
    {
        if (await context.Friendships.AnyAsync()) return;

        var user1Id = await context.Users.Where(u => u.Email == "u1@gmail.com").Select(u => u.Id).FirstAsync();
        var user2Id = await context.Users.Where(u => u.Email == "u2@gmail.com").Select(u => u.Id).FirstAsync();
        var user3Id = await context.Users.Where(u => u.Email == "u3@gmail.com").Select(u => u.Id).FirstAsync();

        Friendship u1u2 = new()
        {
            UserId = user1Id,
            FriendId = user2Id,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
            IsAccepted = true
        };

        Friendship u1u3 = new()
        {
            UserId = user1Id,
            FriendId = user3Id,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
            IsAccepted = true
        };

        Friendship u2u3 = new()
        {
            UserId = user2Id,
            FriendId = user3Id,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
            IsAccepted = true
        };

        context.Friendships.Add(u1u2);
        context.Friendships.Add(u1u3);
        context.Friendships.Add(u2u3);
        await context.SaveChangesAsync();
    }
}
