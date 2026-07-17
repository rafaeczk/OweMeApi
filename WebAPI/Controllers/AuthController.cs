using Application.Common.Extensions;
using Application.Modules.Auth.SignIn;
using Application.Modules.Auth.SignUp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp(SignUpDTO dto)
    {
        var result = await _mediator.Send(new SignUpCommand(dto.Email, dto.FullName, dto.Password));

        return result.ToActionResult();
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<string>> SignIn(SignInDTO dto)
    {
        var result = await _mediator.Send(new SignInCommand(dto.Email, dto.Password));

        if (result.IsSuccess)
        {
            string token = result.Value!;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(2),
                Path = "/"
            };
            HttpContext.Response.Cookies.Append("auth_token", token, cookieOptions);
        }

        return result.ToActionResult();
    }

    [HttpDelete("log-out")]
    [Authorize]
    public async Task<ActionResult> LogOut()
    {
        HttpContext.Response.Cookies.Delete("auth_token", new CookieOptions
        {
            Path = "/"
        });

        return NoContent();
    }
}
