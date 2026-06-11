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
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetMe()
        {
            var result = await _mediator.Send(new GetMeQuery(User.Id()));

            return result.ToActionResult(HttpContext);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrModerator")]
        public async Task<ActionResult<List<UserDTO>>> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());

            return result.ToActionResult(HttpContext);
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = "AdminOrModerator")]
        public async Task<ActionResult<UserDTO>> EditUser(Guid userId, [FromBody] EditUserDTO dto)
        {
            var result = await _mediator.Send(new EditUserCommand(userId, dto.Email, dto.FullName, dto.RoleCode));

            return result.ToActionResult(HttpContext);
        }

        [HttpPut("{userId}/password")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> ChangeUserPassword(Guid userId, [FromBody] ChangeUserPasswordDTO dto)
        {
            var result = await _mediator.Send(new ChangeUserPasswordCommand(userId, dto.Password));

            return result.ToActionResult(HttpContext);
        }
    }
}
