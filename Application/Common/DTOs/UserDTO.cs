using Domain.Enums;

namespace Application.Common.DTOs;

public class UserRoleDTO(string code)
{
    public string Code { get; set; } = code;
    public string Label { get; set; } = code switch
    {
        UserRole.Admin => "Admin",
        UserRole.User => "Użytkownik",
        _ => "Rola nieznana"
    };
}

public record UserDTO(Guid Id, string Email, string FullName, UserRoleDTO Role);
