using Application.Common.Interfaces;
using Application.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Contexts;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid Id =>
        (accessor.HttpContext?.User.Id()) ?? throw new UnauthorizedAccessException("HTTP Context is missing in Headers or user is unauthorized.");

    public string Role =>
        (accessor.HttpContext?.User.Role()) ?? throw new UnauthorizedAccessException("HTTP Context is missing in Headers or user is unauthorized.");
}
