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
        public async Task<ActionResult<string>> SignUp(UserSignUpDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("This email is already in use.");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            User user = new()
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FullName = dto.FullName,
                Hash = hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<string>> SignIn(UserSignInDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Hash))
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
            return NoContent();
        }
    }
}
