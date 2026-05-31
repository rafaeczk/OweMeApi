using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Data.Entities;

public class FriendCode
{
    [Key]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(6)]
    public required string Code { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }
}
