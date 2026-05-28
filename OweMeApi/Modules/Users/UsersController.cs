using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Modules.Users.Dtos;
using OweMeApi.Modules.UserRoles;
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
        [Authorize]
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
        //[Authorize(Roles = "ADMIN")]
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
    }
}
