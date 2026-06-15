using OweMeApi.Modules.Users.Domain.Enums;

namespace OweMeApi.Modules.Users.Features;

public class UserRoleDTO(string code)
{
    public string Code { get; set; } = code;
    public string Label { get; set; } = code switch
    {
        UserRole.Admin => "Admin",
        UserRole.Moderator => "Moderator",
        UserRole.User => "Użytkownik",
        UserRole.Locked => "Zablokowany",
        _ => "Rola nieznana"
    };
}

public record UserDTO(Guid Id, string Email, string FullName, UserRoleDTO Role);
