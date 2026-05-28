using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Modules.Users.Dtos;
using System.Security.Claims;

namespace OweMeApi.Modules.Users
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound("User not found");

            return Ok(new UserDTO(user.Id, user.Email, user.FullName));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users =  await _context.Users
                .Select(u => new UserDTO(u.Id, u.Email, u.FullName))
                .ToListAsync();

            return Ok(users);
        }
    }
}
