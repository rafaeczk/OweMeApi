using OweMeApi.Data.Entities;
using OweMeApi.Modules.UserRoles.Dtos;

namespace OweMeApi.Modules.Users.Features;

public record UserDTO(Guid Id, string Email, string FullName, UserRoleDTO Role);
