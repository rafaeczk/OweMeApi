namespace Domain.Entities;

public class FriendCode
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }

    private FriendCode() { }

    public static FriendCode ForUser(Guid userId)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        var friendCode = new FriendCode()
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(12),
            Code = new string([.. Enumerable.Repeat(chars, 6).Select(s => s[Random.Shared.Next(s.Length)])])
        };

        return friendCode;
    }
}
