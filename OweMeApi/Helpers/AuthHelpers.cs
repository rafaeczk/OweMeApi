using System.Security.Claims;

namespace OweMeApi.Helpers;

public class AuthHelpers
{
    public static (bool, Guid) GetUserId(ClaimsPrincipal User)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        bool userIdOk = Guid.TryParse(userIdString, out Guid userId);

        return (userIdOk, userId);
    }
}

