using OweMeApi.Data;
using OweMeApi.Data.Entities;

namespace OweMeApi.Modules.Friends;

public class FriendsService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task CreateFriendship(Guid userId, Guid friendId)
    {
        var friendship = new Friendship()
        {
            UserId = userId,
            FriendId = friendId,
            IsAccepted = true,
            AcceptedAt = DateTime.UtcNow,
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();
    }

    public async Task CreateFriendshipRequest(Guid userId, Guid friendId)
    {
        var friendship = new Friendship()
        {
            UserId = userId,
            FriendId = friendId,
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();
    }
}
