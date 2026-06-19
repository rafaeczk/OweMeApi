using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FullName { get; set; }
}
