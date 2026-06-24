using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;

    public User(Guid id, string email, string fullName)
    {
        Id = id;
        Email = email;
        FullName = fullName;
    }

    public void UpdateProfile(string email, string fullName)
    {
        Email = email;
        FullName = fullName;
    }
}
