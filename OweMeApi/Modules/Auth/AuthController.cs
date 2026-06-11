using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Modules.Auth.Features.SignIn;
using OweMeApi.Modules.Auth.Features.SignUp;

namespace OweMeApi.Modules.Auth
{
    [Route("api")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("sign-up")]
        public async Task<ActionResult<SignUpResponseDTO>> SignUp(SignUpDTO dto)
        {
            var result = await _mediator.Send(new SignUpCommand(dto.Email, dto.FullName, dto.Password));

            return result.ToActionResult();
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<string>> SignIn(SignInDTO dto)
        {
            var result = await _mediator.Send(new SignInCommand(dto.Email, dto.Password));

            var token = result.Value;

            if (result.IsSuccess && !string.IsNullOrEmpty(token))
            {
                Response.Cookies.Append(AuthService.CookieName, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // TODO: do zmiany
                    SameSite = SameSiteMode.Strict
                });

                return Ok(token);
            }

            return result.ToActionResult();
        }

        [HttpDelete("log-out")]
        [Authorize]
        public async Task<ActionResult> LogOut()
        {
            Response.Cookies.Delete(AuthService.CookieName);
            return NoContent();
        }
    }
}
