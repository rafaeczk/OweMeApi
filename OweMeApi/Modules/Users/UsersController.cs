using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Extensions;
using OweMeApi.Modules.UserRoles.Dtos;
using OweMeApi.Modules.Users.Dtos;

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
            var userId = User.GetUserId();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            return Ok(new UserDTO(
                user.Id,
                user.Email,
                user.FullName,
                new UserRoleDTO(user.RoleCode, user.Role?.Label)
            ));
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrModerator")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users
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
        [Authorize(Policy = "AdminOrModerator")]
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
        [Authorize(Policy = "Admin")]
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
