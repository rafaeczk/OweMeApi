using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Modules.Users.Dtos;
using System.Security.Claims;
using OweMeApi.Modules.UserRoles.Dtos;

namespace OweMeApi.Modules.Users
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet("me")]
        [Authorize(Roles = "LOCKED,USER,MODERATOR,ADMIN")]
        public async Task<ActionResult<UserDTO>> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound("User not found");

            return Ok(
                new UserDTO(
                    user.Id, 
                    user.Email, 
                    user.FullName, 
                    new UserRoleDTO(user.RoleCode, user.Role?.Label)
                    )
                );
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,MODERATOR")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users =  await _context.Users
                .Select(u => 
                    new UserDTO(
                        u.Id, 
                        u.Email, 
                        u.FullName, 
                        new UserRoleDTO(u.RoleCode, u.Role!.Label)
                        )
                    )
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "ADMIN,MODERATOR")]
        public async Task<ActionResult<UserDTO>> EditUser(Guid userId, [FromBody] EditUserDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            if (!(await _context.UserRoles.AnyAsync(r => r.Code == dto.RoleCode)))
                return BadRequest("Wrong RoleCode");

            user.Email = dto.Email;
            user.FullName = dto.Fullname;
            user.RoleCode = dto.RoleCode;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("DB error");
            }

            return NoContent();
        }

        [HttpPut("{userId}/password")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ChangeUserPassword(Guid userId, [FromBody] ChangeUserPasswordDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            user.Hash = hash;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("DB error");
            }

            return NoContent();
        }
    }
}
