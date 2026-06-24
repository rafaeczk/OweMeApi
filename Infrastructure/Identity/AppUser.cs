using Microsoft.AspNetCore.Identity;
using Domain.Entities;

namespace Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public new required string Email { get; set; }

    public static implicit operator User(AppUser user)
    {
        return new User(user.Id, user.Email, user.FullName);
    }
}
