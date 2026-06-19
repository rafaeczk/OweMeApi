using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<string?> GetUserName(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user?.FullName;
    }

    public async Task<string?> GetUserEmail(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user?.Email;
    }

    public async Task<(bool, string)> CreateUser(string email, string fullName, string password)
    {
        ApplicationUser user = new()
        {
            Email = email,
            UserName = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.Succeeded, user.Id.ToString());
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }
}
