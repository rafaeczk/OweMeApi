using Microsoft.AspNetCore.Identity;
using Domain.Entities;

namespace Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;

    public AppUser() { }

    public static implicit operator User(AppUser user)
    {
        return new User(user.Id, user.Email!, user.FullName);
    }
    public static implicit operator AppUser(User user)
    {
        return new AppUser()
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }
}
