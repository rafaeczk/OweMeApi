namespace Domain.Entities;

public class Friendship
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsAccepted { get; set; } = false;
    public DateTime? AcceptedAt { get; set; }

    public User User { get; set; } = null!;
    public User Friend { get; set; } = null!;
}
