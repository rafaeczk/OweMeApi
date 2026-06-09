using System.Security.Claims;

namespace OweMeApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid Id(this ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("User is not signed in or there is no user id");

            return userId;
        }

        public static string Role(this ClaimsPrincipal user)
        {
            var userRole = user.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userRole))
                throw new UnauthorizedAccessException("User is not signed in or there is no user role");

            return userRole;
        }
    }
}
