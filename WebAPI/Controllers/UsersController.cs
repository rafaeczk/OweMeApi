using Application.Common.DTOs;
using Application.Common.Pagination;
using Application.Modules.Users.ChangeUserPassword;
using Application.Modules.Users.GetMe;
using Application.Modules.Users.GetUsers;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> GetMe()
    {
        var result = await _mediator.Send(new GetMeQuery());

        return result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.Admin)]
    public async Task<ActionResult<PagedResult<UserDTO>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetUsersQuery(new(pageNumber, pageSize)));

        return result.ToActionResult();
    }

    [HttpPut("{userId}/password")]
    [Authorize(Policy = AuthPolicies.Admin)]
    public async Task<ActionResult> ChangeUserPassword(Guid userId, [FromBody] ChangeUserPasswordDTO dto)
    {
        var result = await _mediator.Send(new ChangeUserPasswordCommand(userId, dto.Password));

        return result.ToActionResult();
    }
}
