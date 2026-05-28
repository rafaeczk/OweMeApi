using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Modules.Users.Dtos;

namespace OweMeApi.Modules.Auth
{
    [Route("api")]
    [ApiController]
    public class AuthController(AppDbContext context, AuthService authService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly AuthService _authService = authService;

        [HttpPost("sign-up")]
        public async Task<ActionResult<string>> SignUp(UserSignUpDTO request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("This email is already in use.");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            User user = new()
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                Hash = hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<string>> SignIn(UserSignInDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Hash))
                return BadRequest("Wrong email or password");

            var token = _authService.CreateToken(user);

            Response.Cookies.Append(AuthService.CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // DO ZMIANY
                SameSite = SameSiteMode.Strict
            });

            return Ok(token);
        }

        [HttpDelete("log-out")]
        [Authorize]
        public async Task<ActionResult> LogOut()
        {
            Response.Cookies.Delete(AuthService.CookieName);
            return Ok();
        }
    }
}
