using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
