using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string RoleCode { get; set; } = UserRole.User;
}
