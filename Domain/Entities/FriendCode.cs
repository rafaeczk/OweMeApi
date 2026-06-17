namespace Domain.Entities;

public class FriendCode
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
}
