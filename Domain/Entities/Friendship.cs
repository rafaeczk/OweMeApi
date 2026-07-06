using Domain.Exceptions;

namespace Domain.Entities;

public class Friendship
{
    public Guid UserId { get; private set; }
    public Guid FriendId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsAccepted { get; private set; } = false;
    public DateTime? AcceptedAt { get; private set; }

    public User User { get; private set; } = null!;
    public User Friend { get; private set; } = null!;

    public static Friendship FromFriendCode(Guid userId, FriendCode friendCode)
    {
        if (friendCode.ExpiresAt < DateTime.UtcNow)
            throw new FriendCodeExpiredException();

        if (friendCode.UserId == userId)
            throw new YourFriendCodeException();

        return new()
        {
            UserId = userId,
            FriendId = friendCode.UserId,
            IsAccepted = true,
            AcceptedAt = DateTime.UtcNow
        };
    }

    public static Friendship RequestFriendship(Guid userId, Guid friendId)
    {
        return new()
        {
            UserId = userId,
            FriendId = friendId,
        };
    }

    public void Accept(Guid userId)
    {
        if (userId == UserId)
            throw new SelfFriendshipOperationException();

        IsAccepted = true;
        AcceptedAt = DateTime.UtcNow;
    }

    public bool EnsureCanBeDeleted(Guid userId)
    {
        if (userId == UserId)
            throw new SelfFriendshipOperationException();

        return true;
    }
}
