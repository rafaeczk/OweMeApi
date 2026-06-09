using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Data;
using OweMeApi.Extensions;
using OweMeApi.Modules.Users.Features;
using OweMeApi.Modules.Users.Features.ChangeUserPassword;
using OweMeApi.Modules.Users.Features.EditUser;
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
            var result = await _mediator.Send(new EditUserCommand(userId, dto.Email, dto.Fullname, dto.RoleCode));

            return result.ToActionResult();
        }

        [HttpPut("{userId}/password")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> ChangeUserPassword(Guid userId, [FromBody] ChangeUserPasswordDTO dto)
        {
            var result = await _mediator.Send(new ChangeUserPasswordCommand(userId, dto.Password));

            return result.ToActionResult();
        }
    }
}
