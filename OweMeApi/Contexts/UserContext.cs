using OweMeApi.Extensions;

namespace OweMeApi.Contexts;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid Id
    {
        get
        {
            var user = accessor.HttpContext?.User;

            if (user == null)
                throw new UnauthorizedAccessException("HTTP Context is missing in Headers or user is unauthorized.");

            return user.Id();
        }
    }
    public string Role
    {
        get
        {
            var user = accessor.HttpContext?.User;

            if (user == null)
                throw new UnauthorizedAccessException("HTTP Context is missing in Headers or user is unauthorized.");

            return user.Role();
        }
    }
}
