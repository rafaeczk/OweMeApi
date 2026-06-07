using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OweMeApi.Data;
using OweMeApi.Extensions;
using OweMeApi.Modules.Users.Dtos;
using OweMeApi.Modules.Users.Features.GetMe;
using OweMeApi.Modules.Users.Features.GetUsers;

namespace OweMeApi.Modules.Users
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(AppDbContext context, IMediator mediator) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IMediator _mediator = mediator;

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetMe()
        {
            var result = await _mediator.Send(new GetMeQuery(User.GetUserId()));

            return result.ToActionResult();
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrModerator")]
        public async Task<ActionResult<List<UserDTO>>> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());

            return result.ToActionResult();
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
